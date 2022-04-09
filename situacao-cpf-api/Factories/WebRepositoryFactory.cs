namespace situacao_cpf_api.Factories;

public class WebRepositoryFactory
{
    private readonly ChromeDriverFactory chromeFactory;

    public WebRepositoryFactory(ChromeDriverFactory chromeFactory)
    {
        this.chromeFactory = chromeFactory;
    }

    public IWebRepository GetWebRepository()
    {
        return new WebRepository(chromeFactory);
    }
}
