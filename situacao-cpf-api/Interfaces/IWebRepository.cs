namespace situacao_cpf_api.Interfaces;

public interface IWebRepository : IDisposable
{
    ValueTask<IWebElement> GetElement(By elementLocator, bool verifyExistence = true);
    ValueTask<IReadOnlyCollection<IWebElement>> GetElements(By elementLocator, bool verifyExistence = true);
    void Navigate(string URL);
    void WaitUntilElementExists(By elementLocator);
    ValueTask ClickOnElement(By elementLocator, bool verifyExistence = true);
    ValueTask InsertTextOnElement(By elementLocator, string text, bool verifyExistence = true);
    ValueTask<string> GetElementContent(By elementLocator, bool verifyExistence = true);
    void StopDriver();
    string GetCurrentURL();
    void RefreshPage();
    bool ElementExists(By elementLocator);
    void WaitUntilElementIsVisible(By elementLocator);
    ValueTask<IWebElement> GetLastEvent(By classEventTab);
    T RunJavascript<T>(string script);
    ValueTask SwitchToFrame(By frame);
    void SwitchToDefault();
    void DisposeDriver();
    bool IsDisposed();
}
