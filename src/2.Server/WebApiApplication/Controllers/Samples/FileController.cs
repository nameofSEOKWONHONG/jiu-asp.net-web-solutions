using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Infrastructure.Compression;
using DocumentFormat.OpenXml.Drawing.Charts;
using Domain.Entities;
using eXtensionSharp;
using Infrastructure.Persistence;
using Infrastructure.Services.Account;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Size = System.Drawing.Size;

namespace WebApiApplication.Controllers;

public class FileController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    public FileController(ApplicationDbContext context)
    {
        _dbContext = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> Download()
    {
        var urls = new[]
        {
            "https://www.naver.com",
            "https://www.daum.net/",
            "https://www.gmarket.co.kr/"
        };
        
        var list = new ConcurrentBag<InMemoryFile>();
        urls.xForEachAsync(async url =>
        {
            using (var converter = new BrowserImageConverter(url,
                       new PrintOptions()
                       {
                           Orientation = PrintOrientation.Portrait,
                           OutputBackgroundImages = true,
                           ScaleFactor = .8,
                           ShrinkToFit = true
                       }
                   ))
            {
                var filename = converter.Convert(ENUM_IMG_CONVERT_TYPE.PNG); 
                list.Add(filename.xLoadFromFile());
            }
        });

        var result = await list.xToArchiveAsync(bytes =>
        {
            ZstdCompress compress = new ZstdCompress();
            return compress.Compress(bytes);
        });
        
        //create pdf or png
        return File(result, "application/octet-stream", $"{Guid.NewGuid()}.zip");
    }
}

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

    public string Convert(ENUM_IMG_CONVERT_TYPE type)
    {
        _driver.Navigate().GoToUrl(_url);
        var waiter = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var isLoaded = waiter.Until(d => d.Url.Contains(_url));
        if (!isLoaded) throw new Exception("web drive navigating failed.");
        Thread.Sleep(1000);
        this.LoadDynamicContents();

        var filename = string.Empty;
        if(type == ENUM_IMG_CONVERT_TYPE.PDF)
            filename = this.SaveToImage();
        else
            filename = this.SaveToPdf();
        
        _driver.Quit();

        return filename;
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
    private string SaveToPdf()
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
        var filename = $"d:/{Guid.NewGuid().ToString("N")}.pdf";
        _driver.Print(_options).SaveAsFile(filename);
        return filename;
    }

    /// <summary>
    /// save to image
    /// </summary>
    private string SaveToImage()
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
        var filename = $"D://{Guid.NewGuid()}.png";
        _driver.GetScreenshot().SaveAsFile(filename, ScreenshotImageFormat.Png);

        return filename;

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

public enum ENUM_IMG_CONVERT_TYPE
{
    PDF,
    PNG
}

