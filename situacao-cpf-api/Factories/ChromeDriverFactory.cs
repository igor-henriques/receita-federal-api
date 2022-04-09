namespace situacao_cpf_api.Factories;

public class ChromeDriverFactory
{
    public ChromeDriver GetChromeDriver()
    {
        string userDataDir = Environment.GetEnvironmentVariable("USER_DATA_DIR");
        string driverPath = Environment.GetEnvironmentVariable("CHROMEDRIVER_PATH");

        ChromeDriverService driverService = driverPath is null ? ChromeDriverService.CreateDefaultService() : ChromeDriverService.CreateDefaultService(driverPath);

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

        if (!string.IsNullOrEmpty(userDataDir))
            options.AddArgument($"user-data-dir={userDataDir}");

        options.AddExcludedArgument("enable-automation");
        options.AddExcludedArgument("--headless");

        options.AddAdditionalOption("useAutomationExtension", false);

        options.BinaryLocation = Environment.GetEnvironmentVariable("GOOGLE_CHROME_BIN");
        options.AddExtension(Path.Combine(Directory.GetCurrentDirectory(), "hcaptcha-solver.crx"));
        options.AddExtension(Path.Combine(Directory.GetCurrentDirectory(), "cssblock.crx"));
        options.AddExtension(Path.Combine(Directory.GetCurrentDirectory(), "blockimage.crx"));

        return new ChromeDriver(driverService, options);
    }
}
