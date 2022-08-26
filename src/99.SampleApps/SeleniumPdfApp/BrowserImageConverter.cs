using System.Drawing;
using System.Drawing.Imaging;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using WDSE;
using WDSE.Decorators;
using WDSE.ScreenshotMaker;

namespace SeleniumPdfApp;

public class BrowserImageConverter : IDisposable
{
    private readonly string _url;
    private readonly EdgeOptions _edgeOptions;
    private readonly WebDriver _driver;
    private readonly PrintOptions _options;

    public BrowserImageConverter(string url, PrintOptions options = null)
    {
        _url = url;
        _edgeOptions = new EdgeOptions();
        _edgeOptions.AddArguments("--headless", "--disable-gpu", "--run-all-compositor-stages-before-draw");
        _driver = new EdgeDriver(_edgeOptions);
        _options = options;
        if (_options is null) _options = new PrintOptions();
    }

    public void Convert()
    {
        _driver.Navigate().GoToUrl(_url);
        var waiter = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var isLoaded = waiter.Until(d => d.Url.Contains(_url));
        if (!isLoaded) throw new Exception("web drive navigating failed.");
        Thread.Sleep(1000);
        this.LoadDynamicContents();
        this.SaveToImage();
        Thread.Sleep(1000);
        this.SaveToPdf();
        _driver.Quit();
    }
    
    /// <summary>
    /// lazy loading되는 이미지를 미리 불러오기 위해 설정함.
    /// </summary>
    private void LoadDynamicContents()
    {
        var fullWidth = (long)_driver.ExecuteScript("return document.body.parentNode.scrollWidth");
        var fullHeight = (long)_driver.ExecuteScript("return document.body.parentNode.scrollHeight");
        _driver.Manage().Window.Size = new Size(System.Convert.ToInt32(fullWidth), System.Convert.ToInt32(fullHeight));
        var filename = $"D://{Guid.NewGuid()}.png";
        _driver.GetScreenshot().SaveAsFile(filename, ScreenshotImageFormat.Png);
        File.Delete(filename);
    }

    /// <summary>
    /// save to pdf
    /// </summary>
    private void SaveToPdf()
    {
        // var lastHeight = (long)driver.ExecuteScript("return document.body.scrollHeight");
        // while (true)
        // {
        //     driver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
        //     Thread.Sleep(1000);
        //     var newHeight = (long)driver.ExecuteScript("return document.body.scrollHeight");
        //     if(newHeight <= lastHeight) break;
        //     lastHeight = newHeight;
        // }
        _driver.Print(_options).SaveAsFile($"d:/{Guid.NewGuid().ToString("N")}.pdf");
    }

    /// <summary>
    /// save to image
    /// </summary>
    private void SaveToImage()
    {
        #region [impl 1]

        // var screenshot = driver.TakeScreenshot(new VerticalCombineDecorator(new ScreenshotMaker()));
        // using (var ms = new MemoryStream(screenshot))
        // {
        //     Bitmap bmp = new Bitmap(ms);
        //     bmp.Save($"D://{Guid.NewGuid()}.png", ImageFormat.Png);
        // }

        #endregion

        #region [impl 2]

        var fullWidth = (long)_driver.ExecuteScript("return document.body.parentNode.scrollWidth");
        var fullHeight = (long)_driver.ExecuteScript("return document.body.parentNode.scrollHeight");
        _driver.Manage().Window.Size = new Size(System.Convert.ToInt32(fullWidth), System.Convert.ToInt32(fullHeight));
        _driver.GetScreenshot().SaveAsFile($"D://{Guid.NewGuid()}.png", ScreenshotImageFormat.Png);

        #endregion

    }

    public void Dispose()
    {
        if (_driver is not null)
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}