namespace situacao_cpf_api.Services;

public class ReceitaFederalService : IReceitaFederalService
{
    private readonly IWebRepository webContext;

    private readonly Timer timeoutController;
    private bool timeoutControllerFlag = false;

    private SituacaoCadastralRequest bakRequest;

    public ReceitaFederalService(IWebRepository webContext)
    {
        this.webContext = webContext;
        this.timeoutController = new Timer(async (obj) => await TimeoutSafetyVerifier(), null, 10_000, 10_000);
    }

    private async Task TimeoutSafetyVerifier()
    {
        try
        {
            if (!timeoutControllerFlag)
            {
                timeoutControllerFlag = true;

                await ObterSituacaoCadastral(bakRequest);

                timeoutControllerFlag = false;
            }                            
        }
        catch { }
    }

    public async ValueTask<SituacaoCadastralResponse> ObterSituacaoCadastral(SituacaoCadastralRequest request)
    {
        this.bakRequest = request;

        try
        {
            await CompleteFields(request);

            await ResolveCaptcha();

            await ClickSubmit(request);

            await WaitForResultPageLoad();
        }
        catch
        {
            await ObterSituacaoCadastral(request);
        }

        return await ObterDados();
    }

    private async Task CompleteFields(SituacaoCadastralRequest request)
    {
        try
        {
            webContext.Navigate(ReceitaFederalElements.ReceitaFederalURL);

            await webContext.InsertTextOnElement(By.XPath(ReceitaFederalElements.CpfTextbox), request.CPF!);

            await webContext.InsertTextOnElement(By.XPath(ReceitaFederalElements.DtNascimentoTextbox), request.DtNascimento!.Date.ToShortDateString());
        }
        catch
        {
            await ObterSituacaoCadastral(request);
        }
    }

    private async Task ResolveCaptcha()
    {
        await webContext.SwitchToFrame(By.XPath("//*[@id=\"hcaptcha\"]/iframe"));

        await TaskUtils.WaitWhile(() => webContext.GetElement(By.Id("checkbox"), false).Result.GetAttribute("aria-checked") == "false");

        webContext.SwitchToDefault();
    }

    private async Task ClickSubmit(SituacaoCadastralRequest request)
    {
        await webContext.ClickOnElement(By.XPath(ReceitaFederalElements.SubmitButton));

        if (webContext.ElementExists(By.XPath(ReceitaFederalElements.ValidacaoErrorMessage)))
            await ObterSituacaoCadastral(request);
    }

    private async ValueTask WaitForResultPageLoad()
    {
        await Task.Run(() => webContext.WaitUntilElementIsVisible(By.Id("portal-title")));
    }

    private async ValueTask<bool> IsDataWrong()
    {
        try
        {
            var errorContent = await webContext.GetElementContent(By.XPath("/html/body/div[2]/div[2]/div[1]/div/div/div/div/div[1]/span/h4"), false);

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
            throw new ArgumentException("'CPF' e/ou 'data de nascimento' incorretos");
        }

        SituacaoCadastralResponse response = new();

        var painelConteudos = await webContext.GetElements(By.ClassName("clConteudoEsquerda"));

        var painelDados = painelConteudos.FirstOrDefault().FindElements(By.ClassName("clConteudoDados"));

        var painelComprovante = painelConteudos.LastOrDefault().FindElements(By.ClassName("clConteudoComp"));

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
