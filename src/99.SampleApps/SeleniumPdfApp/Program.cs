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
    "https://logine.ecount.com/ECERP/EGM/EGM026M?vrqa=N10KJdtXvBxuz%2BlhUbfIXEy16lq16JI5LZYvmtEdo%2FmFOMMWxxbGoK%2FjvQyH7MFtfZwt%2FQ9xCDpBddJXbTQWzA%3D%3D&vrqb=54555a4c5d4551565a415b41544d561156125c520b145a12570158115e40070559450d4353560d445640525757434044&vrqc=1",
    //"https://www.naver.com",
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
 
