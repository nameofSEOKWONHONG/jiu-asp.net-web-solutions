using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace SeleniumPdfApp;

public class SeleniumSaveToPdfProvider
{
    private readonly string _url;
    private readonly EdgeOptions _edgeOptions;
    public SeleniumSaveToPdfProvider(string url)
    {
        _url = url;
        _edgeOptions = new EdgeOptions();
        _edgeOptions.AddArguments("--headless", "--disable-gpu", "--run-all-compositor-stages-before-draw");        
    }

    public void SaveToPdf(PrintOptions options)
    {
        using (WebDriver driver = new EdgeDriver(_edgeOptions))
        {
            driver.Navigate().GoToUrl(_url);
            var waiter = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var isLoaded = waiter.Until(d => d.Url.Contains(_url));
            if (!isLoaded) throw new Exception("web drive navigating failed.");
            Thread.Sleep(5000);
            // var lastHeight = (long)driver.ExecuteScript("return document.body.scrollHeight");
            // while (true)
            // {
            //     driver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            //     Thread.Sleep(1000);
            //     var newHeight = (long)driver.ExecuteScript("return document.body.scrollHeight");
            //     if(newHeight <= lastHeight) break;
            //     lastHeight = newHeight;
            // }
            driver.Print(options).SaveAsFile($"d:/{Guid.NewGuid().ToString("N")}.pdf");
            driver.Quit();
        }
    }
}