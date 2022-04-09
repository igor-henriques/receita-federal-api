namespace situacao_cpf_api.Services;

public class ReceitaFederalService : IReceitaFederalService, IDisposable
{
    private IWebRepository webContext;
    private readonly ILogger<ReceitaFederalService> logger;
    private readonly WebRepositoryFactory webContextFactory;
    private SituacaoCadastralRequest bakRequest;
    private Timer timeoutController;

    public ReceitaFederalService(ILogger<ReceitaFederalService> logger, IWebRepository webContext, WebRepositoryFactory webContextFactory)
    {
        this.webContext = webContext;
        this.webContextFactory = webContextFactory;
        this.timeoutController = new Timer(async (obj) => await TimeoutSafetyVerifier(), null, 12_000, Timeout.Infinite);
        this.logger = logger;
    }

    private async Task TimeoutSafetyVerifier()
    {
        webContext?.DisposeDriver();

        webContext = null;

        CleanDriverGarbage.Run();

        await ObterSituacaoCadastral(bakRequest);
    }

    public async ValueTask<SituacaoCadastralResponse> ObterSituacaoCadastral(SituacaoCadastralRequest request)
    {
        RequestQueueController.Busy = true;

        PrepareProperties(request);

        try
        {
            await CompleteFields(request);

            await ResolveCaptcha();

            await ClickSubmit(request);

            WaitForResultPageLoad();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex.ToString());

            await TimeoutSafetyVerifier();
        }

        return await ObterDados();
    }

    private void PrepareProperties(SituacaoCadastralRequest request)
    {
        this.bakRequest = request;        

        webContext = webContext ?? webContextFactory.GetWebRepository();
    }

    private async Task CompleteFields(SituacaoCadastralRequest request)
    {
        if (!webContext.GetCurrentURL().Equals(ReceitaFederalElements.ReceitaFederalURL))
            webContext.Navigate(ReceitaFederalElements.ReceitaFederalURL);
        else
            webContext.RefreshPage();

        await webContext.InsertTextOnElement(By.XPath(ReceitaFederalElements.CpfTextbox), request.CPF!);

        await webContext.InsertTextOnElement(By.XPath(ReceitaFederalElements.DtNascimentoTextbox), request.DtNascimento!.Date.ToShortDateString());
    }

    private async Task ResolveCaptcha()
    {
        await webContext.SwitchToFrame(By.XPath(ReceitaFederalElements.CaptchaFrame));

        await TaskUtils.WaitWhile(() => webContext.GetElement(By.Id(ReceitaFederalElements.IdCheckbox), false).Result.GetAttribute("aria-checked") == "false");

        webContext.SwitchToDefault();
    }

    private async Task ClickSubmit(SituacaoCadastralRequest request)
    {
        await webContext.ClickOnElement(By.XPath(ReceitaFederalElements.SubmitButton));

        if (webContext.ElementExists(By.XPath(ReceitaFederalElements.ValidacaoErrorMessage)))
            await ObterSituacaoCadastral(request);
    }

    private void WaitForResultPageLoad()
    {
        webContext.WaitUntilElementIsVisible(By.Id(ReceitaFederalElements.IdTituloPortal));
    }

    private async ValueTask<bool> IsDataWrong()
    {
        try
        {
            var errorContent = await webContext.GetElementContent(By.XPath(ReceitaFederalElements.XPathMensagemErro), false);

            return !string.IsNullOrEmpty(errorContent);
        }
        catch
        {
            return false;
        }
    }

    private async ValueTask<SituacaoCadastralResponse> ObterDados()
    {
        if (await IsDataWrong())
        {
            await timeoutController.DisposeAsync();

            throw new ArgumentException("'CPF' e/ou 'data de nascimento' incorretos");
        }

        SituacaoCadastralResponse response = new();

        var painelConteudos = await webContext.GetElements(By.ClassName(ReceitaFederalElements.ClassePainelConteudo));

        var painelDados = painelConteudos.FirstOrDefault().FindElements(By.ClassName(ReceitaFederalElements.ClassePainelDados));

        var painelComprovante = painelConteudos.LastOrDefault().FindElements(By.ClassName(ReceitaFederalElements.ClassePainelComprovantes));

        response.CPF = painelDados.ElementAt(0).FindElement(By.TagName("b")).Text;
        response.Nome = painelDados.ElementAt(1).FindElement(By.TagName("b")).Text;
        response.DtNascimento = DateTime.Parse(painelDados.ElementAt(2).FindElement(By.TagName("b")).Text);
        response.SituacaoCadastral = painelDados.ElementAt(3).FindElement(By.TagName("b")).Text;
        response.DtInscricao = DateTime.Parse(painelDados.ElementAt(4).FindElement(By.TagName("b")).Text);
        response.DigitoVerificador = painelDados.ElementAt(5).FindElement(By.TagName("b")).Text;
        response.CodComprovante = painelComprovante.ElementAt(1).FindElement(By.TagName("b")).Text;
        response.HoraConsulta = DateTime.Parse(painelComprovante.ElementAt(0).FindElement(By.TagName("b")).Text);

        await timeoutController.DisposeAsync();

        return response;
    }

    public void Dispose()
    {        
        RequestQueueController.Busy = false;
    }
}
