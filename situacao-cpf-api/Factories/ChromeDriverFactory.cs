namespace situacao_cpf_api.Factories;

public class ChromeDriverFactory
{
    public ChromeDriver GetChromeDriver()
    {
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
        options.AddExtension(Path.Combine(Directory.GetCurrentDirectory(), "cssblock.crx"));
        options.AddExtension(Path.Combine(Directory.GetCurrentDirectory(), "blockimage.crx"));

        return new ChromeDriver(driverService, options);
    }
}
