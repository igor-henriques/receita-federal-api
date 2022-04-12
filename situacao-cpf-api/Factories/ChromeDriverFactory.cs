using Microsoft.Extensions.Options;

namespace situacao_cpf_api.Factories;

public class ChromeDriverFactory
{
    private readonly IOptions<Configuration> configuration;

    public ChromeDriverFactory(IOptions<Configuration> configuration)
    {
        this.configuration = configuration;
    }

    public ChromeDriver GetChromeDriver()
    {
        ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(configuration.Value.DriverPath);

        driverService.HideCommandPromptWindow = true;

        ChromeOptions options = new ChromeOptions();
        options.AddArguments(new List<string>()
        {
            "--no-sandbox",
            "--disable-dev-shm-usage",
            "--disable-blink-features=AutomationControlled",                    
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

        options.AddExcludedArgument("enable-automation");
        options.AddExcludedArgument("--headless");

        options.AddAdditionalOption("useAutomationExtension", false);

        if (configuration.Value.ChromePath.Trim().Length > 1)
            options.BinaryLocation = configuration.Value.ChromePath;

        options.AddExtension(Path.Combine(Directory.GetCurrentDirectory(), "hcaptcha-solver.crx"));
        options.AddExtension(Path.Combine(Directory.GetCurrentDirectory(), "cssblock.crx"));
        options.AddExtension(Path.Combine(Directory.GetCurrentDirectory(), "blockimage.crx"));

        return new ChromeDriver(driverService, options);
    }
}
