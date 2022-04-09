namespace situacao_cpf_api.Services;

public class ReceitaFederalService : IReceitaFederalService
{
    private IWebRepository webContext;
    private readonly ILogger<ReceitaFederalService> logger;
    private readonly WebRepositoryFactory webContextFactory;
    private Timer timeoutController;

    public ReceitaFederalService(ILogger<ReceitaFederalService> logger, IWebRepository webContext, WebRepositoryFactory webContextFactory)
    {
        this.webContext = webContext;
        this.webContextFactory = webContextFactory;
        this.logger = logger;
    }

    private void TimeoutSafetyVerifier()
    {
        CleanDriverGarbage.Run();

        webContext.DisposeDriver();

        webContext = null;
    }

    public async ValueTask<SituacaoCadastralResponse> ObterSituacaoCadastral(SituacaoCadastralRequest request)
    {
        try
        {
            this.timeoutController = new Timer((obj) => TimeoutSafetyVerifier(), null, 15_000, Timeout.Infinite);

            webContext = webContext ?? webContextFactory.GetWebRepository();

            await CompleteFields(request);

            await ResolveCaptcha();

            await ClickSubmit();

            WaitForResultPageLoad();

            var response = await ObterDados();

            webContext.DisposeDriver();

            return response;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex.ToString());

            throw;
        }
    }

    private async Task CompleteFields(SituacaoCadastralRequest request)
    {
        if (!webContext.GetCurrentURL().Equals(ReceitaFederalElements.ReceitaFederalURL))
            webContext?.Navigate(ReceitaFederalElements.ReceitaFederalURL);
        else
            webContext?.RefreshPage();

        await webContext.InsertTextOnElement(By.XPath(ReceitaFederalElements.CpfTextbox), request.CPF!);

        await webContext.InsertTextOnElement(By.XPath(ReceitaFederalElements.DtNascimentoTextbox), request.DtNascimento!.Date.ToShortDateString());
    }

    private async Task ResolveCaptcha()
    {
        await webContext.SwitchToFrame(By.XPath(ReceitaFederalElements.CaptchaFrame));

        await TaskUtils.WaitWhile(() => webContext.GetElement(By.Id(ReceitaFederalElements.IdCheckbox), false).Result.GetAttribute("aria-checked") == "false");

        webContext?.SwitchToDefault();
    }

    private async Task ClickSubmit()
    {
        try
        {
            await webContext.ClickOnElement(By.XPath(ReceitaFederalElements.SubmitButton));
        }
        catch (Exception e) { logger.LogCritical(e.ToString()); }
    }

    private void WaitForResultPageLoad()
    {
        try
        {
            webContext.WaitUntilElementIsVisible(By.Id(ReceitaFederalElements.IdTituloPortal));
        }
        catch (Exception e) { logger.LogCritical(e.ToString()); }
    }

    private async ValueTask<bool> IsDataWrong()
    {
        try
        {
            var errorContent = await webContext.GetElementContent(By.XPath(ReceitaFederalElements.XPathMensagemErro), false);

            return !string.IsNullOrEmpty(errorContent);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex.ToString());

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
}
