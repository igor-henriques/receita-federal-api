Console.WriteLine("CPF:");
var cpf = "06162747778";

Console.WriteLine("Data de Nascimento:");
var dtNascimento = DateTime.Parse("10/01/1998");

CleanDriverGarbage.Run();

ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
driverService.HideCommandPromptWindow = true;

ChromeOptions options = new ChromeOptions();
options.AddArguments(new List<string>()
{
    "--disable-blink-features=AutomationControlled",
    "--disable-dev-shm-usage",
    "--no-sandbox",
    "--disable-impl-side-painting",
    "--disable-setuid-sandbox",
    "--disable-seccomp-filter-sandbox",
    "--disable-breakpad",
    "--disable-client-side-phishing-detection",
    "--disable-cast",
    "--disable-cast-streaming-hw-encoding",
    "--disable-cloud-import",
    "--disable-popup-blocking",
    "--ignore-certificate-errors",
    "--disable-session-crashed-bubble",
    "--disable-ipv6",
    "--allow-http-screen-capture",
    "--start-minimized"
});

options.AddExtension(Path.Combine(Directory.GetCurrentDirectory(), "hcaptcha-solver.crx"));

using (IWebRepository webContext = new WebRepository(new(driverService, options)))
{
    webContext.Navigate("https://servicos.receita.fazenda.gov.br/servicos/cpf/consultasituacao/consultapublica.asp");

    await webContext.InsertTextOnElement(By.XPath("//*[@id=\"txtCPF\"]"), cpf!);

    await webContext.InsertTextOnElement(By.XPath("//*[@id=\"txtDataNascimento\"]"), dtNascimento!.Date.ToShortDateString());

    await Task.Delay(10000);

    await webContext.ClickOnElement(By.XPath("//*[@id=\"id_submit\"]"));

    var situacao = await webContext.GetElementContent(By.XPath("/html/body/div[2]/div[2]/div[1]/div/div/div/div/div/div[1]/div[2]/p/span[4]/b"));

    Console.WriteLine($"A situação do CPF {cpf} está {situacao}");
}    