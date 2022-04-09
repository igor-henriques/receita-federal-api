namespace situacao_cpf_api.Repositories;

public class WebRepository : IWebRepository, IDisposable
{
    private readonly ChromeDriver _driver;

    private readonly WebDriverWait _wait;

    public WebRepository(ChromeDriverFactory driverFactory)
    {
        this._driver = driverFactory.GetChromeDriver();

        this._driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(7);

        this._wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(7));

        this._wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));
    }

    public void Navigate(string URL)
    {
        _driver.Navigate().GoToUrl(URL);
    }

    public async ValueTask<IWebElement> GetElement(By elementLocator, bool verifyExistence = true)
    {
        if (verifyExistence)
            WaitUntilElementExists(elementLocator);

        var element = await ValueTask.FromResult(_driver.FindElement(elementLocator));

        return element;
    }

    public async ValueTask<IWebElement> GetLastEvent(By classEventTab)
    {
        var lastEvent = await GetElements(classEventTab);

        return lastEvent.Last();
    }

    public async ValueTask<IReadOnlyCollection<IWebElement>> GetElements(By elementLocator, bool verifyExistence = true)
    {
        if (verifyExistence)
            WaitUntilElementExists(elementLocator);

        return await ValueTask.FromResult(_driver.FindElements(elementLocator));
    }

    public async ValueTask<string> GetElementContent(By elementLocator, bool verifyExistence = true)
    {
        var element = await GetElement(elementLocator, verifyExistence);

        return await ValueTask.FromResult(element.Text);
    }

    public async ValueTask ClickOnElement(By elementLocator, bool verifyExistence = true)
    {
        var element = await GetElement(elementLocator, verifyExistence);

        element.Click();
    }

    public async ValueTask InsertTextOnElement(By elementLocator, string text, bool verifyExistence = true)
    {
        var element = await GetElement(elementLocator, verifyExistence);

        element.SendKeys(text);
    }

    public void WaitUntilElementExists(By elementLocator)
    {
        MoveToPopUp();

        _wait.Until(_driver =>
        {
            try
            {
                var func = ExpectedConditions.ElementIsVisible(elementLocator);

                var element = func.Invoke(_driver);

                return element != null;
            }
            catch { return false; }
        });
    }

    private void MoveToPopUp()
    {
        _driver.SwitchTo().ActiveElement();
    }

    public void StopDriver()
    {
        _driver?.Close();
    }

    public string GetCurrentURL()
    {
        return _driver.Url;
    }

    public void RefreshPage()
    {
        _driver.Navigate().Refresh();
    }

    public async ValueTask SwitchToFrame(By frame)
    {
        var elementFrame = await GetElement(frame);

        _driver.SwitchTo().Frame(elementFrame);
    }

    public void SwitchToDefault()
    {
        _driver.SwitchTo().DefaultContent();
    }

    public bool ElementExists(By elementLocator)
    {
        try
        {
            return _driver.FindElement(elementLocator) != null;
        }
        catch (Exception) { }

        return false;
    }

    public T RunJavascript<T>(string script)
    {
        IJavaScriptExecutor js = _driver;

        var result = (T)js.ExecuteScript(script);

        return result;
    }

    public void Dispose()
    {
        _driver?.Dispose();
    }

    public void WaitUntilElementIsVisible(By elementLocator)
    {
        try
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(elementLocator));
        }
        catch { }
    }

    public void DisposeDriver()
    {        
        _driver?.Dispose();
    }

    public bool IsDisposed()
    {
        return string.IsNullOrEmpty(_driver?.Title);
    }
}