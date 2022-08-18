// See https://aka.ms/new-console-template for more information

using OpenQA.Selenium;
using SeleniumPdfApp;

/*
 * 전체 컨텐츠에 대하여 서버 랜더링을 사용하는 사이트는 정상적으로 pdf 출력됨.
 * 이외에 lazy-loading과 같은 이미지 로딩 기능은 정상적으로 확인 불가능.
 * 무한 스크롤 사이트도 정상적 pdf 출력 불가능.
 */

var urls = new[]
{
    "https://www.naver.com",
    // "https://www.youtube.com/",
    // "https://www.google.com/",
    // "https://www.daum.net/",
     //"https://www.gmarket.co.kr/",
    //"https://github.com/nameofSEOKWONHONG/jiu-asp.net-web-solutions",
    //"https://github.com/nameofSEOKWONHONG/jiu-asp.net-web-solutions/blob/main/src/1.Share/Application/Script/ScriptInitializer.cs",
    //"http://www.etoland.co.kr/bbs/board.php?bo_table=etohumor05&wr_id=2064684&sca=%C1%A4%BA%B8"
};

foreach (var url in urls)
{
    Console.WriteLine($"navigating {url}");
    var saveProvider = new SeleniumSaveToPdfProvider(url);
    saveProvider.SaveToPdf(new PrintOptions()
    {
        Orientation = PrintOrientation.Portrait,
        OutputBackgroundImages = true,
        ScaleFactor = 1.0,
        ShrinkToFit = true
    });
    Console.WriteLine($"navigated {url}");
}
 
