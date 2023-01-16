# jiu-asp.net-web-solutions

`jiu-asp.net-web-solutions`는 asp.net core 6기준으로 제작하는 서버 및 Blazor Server / WASM 프로젝트 템플릿입니다.  
`
어느 시점에 템플릿을 벗어난 프로젝트가 되었습니다.  
포함된 소스는 참고만 하시고 실제 프로젝트에 이런 코드를 사용할 수 있다 정도로 보시기 바랍니다.
`

이 프로젝트는  
`https://github.com/blazorhero/CleanArchitecture`  
`https://github.com/ljlm0402/typescript-express-starter`  
에 기초하고 있습니다.

프로젝트의 구조는 아래와 같습니다.

## 프로젝트 구조
1. Share  : 공유 가능한 클래스 라이브러리 집합니다.
    - Core : 프로젝트의 핵심 코드 집합입니다.
        - Application
            - Application의 기본 코드 집합니다.
            - 추상적 Base Class 및 기반 인프라코드를 정의합니다.
            - 제작될 모든 Application Project에서 사용합니다.
            - WASM 및 클라인언트 범주에 포함되는 솔루션 및 프로젝트에 사용되지 않습니다.
            - 처음 기획에는 Application이 WASM 과 프론트 서버에도 모두 동작하도록 하는 것이 목표였으나 DBContext가 올라가면서 서버측 동작에서만 사용하는 것으로 변경함.
              즉, 서버측 프로젝트에서만 사용함.
        - Domain
            - Entity, Dto, Enum등의 데이터 기반 코드 집합니다.
            - Database정의 및 전달 Parameter등을 정의 합니다.
            - 서버, WASM 모두 사용 가능.
        - eXtensionSharp
            - static class library의 집합입니다.
            - 서버, WASM 모두 사용 가능.
        - Infrastructure
            - Web Application에 사용될 인프라코드의 집합입니다.
            - `Application`은 `Infrastrcture`를 참조할 수 없습니다.
            - Web Application에 사용될 코드만을 정의 합니다.
            - 서버측만(ASP.NET) 사용가능

    - Project : Application을 정의합니다.
        - 실제 구현할 Application 코드를 정의합니다.
        - Service 및 MediatR등으로 구성됩니다.
        - Infrastructure에 의존성 없어야 함.
        
2. Server : 서버 Application 입니다.
    - Project : `1.Share>Project`를 참조하여 만드는 개별 Web Api Project 입니다.  
    MSA 형식의 구성을 갖을 수 있습니다.
    - WebApiApplication : 모놀리식으로 구성되는 Web Api Application 입니다.
3. Client : Client Application 입니다. Blazor Server, Blazor Wasm 프로젝트 입니다.
    - Share
        - Blazor Server와 Blazor Wasm에서 모두 사용할 클라이언트 비즈니스 로직을 정의합니다.
        - 해당 프로젝트에서는 `1.Share>Core>Infrastructure`를 사용할 수 없습니다.
        - <b>해당 프로젝트에 사용되는 `1.Share>Core>Application`은 제거될 예정입니다.</b>
    - BlazorServerApplication : Blazor Server Application 입니다.
    - BlazorWasmApplication : Blazor Wasm Application 입니다.
    - AvaloniaCrossPlatformApp : Avalonia Application 입니다. 데스크탑 및 웹 프론트 크로스 플랫폼 프로젝트 입니다.
    - SpectreConsoleApplication : SpectreConsole을 사용한 Console Applcation 프로젝트 입니다. 크로스 플랫폼을 지원합니다.

## 프로젝트 설정
1. 외부 프로젝트 clone
   1. eXtensionSharp 프로젝트가 누락되어 있다면 아래의 repository를 clone합니다.
   2. https://github.com/nameofSEOKWONHONG/eXtensionSharp.git
   3. 누락된 프로젝트는 솔루션 폴더의 `1.Share>Core`에 추가합니다.
2. migration 진행
   1. 마이그레이션 진행전에 MSSQL LOCAL DB가 설치 되어 있어야 합니다.
   2. visual studio가 설치되어 있다면 아래의 커맨드로 구동할 수 있습니다.
      1. sqllocaldb start MSSQLLocalDB
      2. MSSQLLocalDB는 기본으로 설치되는 local database 입니다.
   3. 모두 설치 및 구동되고 있다면 아래의 커맨드를 실행합니다.
      1. prompt> dotnet ef migrations add migration20220819 --project "..\..\1.Share\Infrastructure\"
      2. prompt> dotnet ef database update
   4. 만약 WebApplication별로 설정할 경우 Infrastructure의 ApplicationDatabaseContext는 각 Application 단위로 내려와야 합니다.
3. 솔루션을 빌드 합니다.
4. 정상적으로 빌드 되었다면 blazor wasm 솔루션 페이지가 노출됩니다.
5. swagger페이지는 아래와 같이 확인할 수 있습니다.
   1. https://localhost:5001/swagger
6. hangfire페이지는 아래와 같이 확인할 수 있습니다.
   1. https://localhost:5001/jobs
7. 로그 서버로 elk를 사용합니다. 릴리즈 구성에서 작동하지만 필요로 할 경우 `appsettings.Development.json`을 참고합니다. 

## 중요사항
1. `1.Share>Core>Infrastructure`는 Web Api Server Application을 위한 구성입니다.  
따라서 해당 프로젝트는 Web Api Server Application에서만 사용합니다.
2. `Interface`의 정의는 `1.Share>Core>Application>Interfaces`에 작성합니다.  
실제 구현은 각각의 Application 프로젝트에서 담당합니다.
3. Web Api Application에서 Blazor WASM을 호스팅 할 수 있습니다.  
관련 코드는 `2.Server>WebApiApplication>Startup.cs`를 참고 합니다.
4. 현재 구조에는 Blazor Server 및 WASM에서 `1.Core>Application`을 참조하고 있습니다.  
WASM에 포함된 `Application` 관련 코드는 제거 예정이며 WASM에서 사용될 라이브러리 프로젝트는 별도 생성 예정입니다.  4)항목은 진행되어 더 이상 WASM 프로젝트에서 `Application`프로젝트를 참조하지 않습니다.

## 히스토리  
`commit history v1.0`까지는 모놀로식 개발 버전입니다.  
이후 버전은 `MSA` 및 모놀리식 개발을 모두 지원하도록 개발되고 있습니다.

## 이 프로젝트에 포함된 내역
* ORM
    * Entity Framework Core
    * SqlKata
    * ~~SqlSugar~~
    * ~~Chloe~~
* Database & NoSql
    * SQL-Server
    * MYSQL
    * Postgres
    * LiteDB    
    * Redis (KV Store)
    * Microsoft Faster(KV Store) 
* Logging
    * Serilog
    * ELK
* IOC
    * ASP.NET CORE Dependency Injection
* MessageQueue
    * Confluent.Kafka
* Authorize
    * JWT
* Script
    * CS-Script
    * JINT
    * ClearScript
    * IronPython
    * Javascript.Node
* Plugin
    * McMaster.NETCore.Plugins (사용하지 않을 수 있습니다.)
* Http
    * RestSharp (사용하지 않을 수 있습니다.)
* Documataion
    * SwaggerUI
* Encrypt
    * BCrypt.Net-Next
* Scheduler
    * Hangfire
    * Qutarz.Net (워커 어플리케이션에서만 사용)
* Mapper
    * Mapster
* CQRS
    * MediatR
* Email
    * MimeKit
    * MailKit
* Console
    * SpectreConsole
* Desktop Cross Platform
    * Avalonia

## 프로젝트 규칙
* 상수형 자료형은 대문자 표기
* Entity 타입은 대문자 표기
* ENUM 형은 대문자 표기
* 멤버변수는 "_" 표기
* 멤버변수는 "_"이후 소문자 표기
* 표현이 달라지는 시점에서 대소문자 표기
* Sql 관련 표현식은 모두 대문자 표기
* DB 관련 사항은 모두 대문자 표기며 축약형 문자 표기  
Sql 컬럼 및 프로시저는 길이 제한이 있다.
* 함수명은 대문자 표기로 시작

## API 중요 개발 원칙
1. 한번 배포된 API는 변경하지 않는다.
   1. 당연한 이야기.
2. 이전 버전과 호환성을 깨뜨리지 않는다.
   1. API Versioning을 해야하는 이유
3. 고객사례를 API개발에 반영한다.
   1. `1`,`2`의 이야기인 동시에 사용자가 사용하기 편해야 효율적이라는 말.
4. 자체 설명적이고 명확한 목적을 가진 API를 만든다.
   1. `Controller`,`Action`등의 명칭에 명확하게 용도가 드러나도록 해야한다.
5. 명시적이고 문서화되도록 오류를 발생시킨다.
   1. 오류를 발생시킬때 어떻게 해야하는가?
      1. 대부분 Http Connection이 연결된 상태를 바탕으로 오류 메세지를 작성한다.
      2. Http Response 규칙을 따른다. 즉, 400, 500 메세지를 보낸다는 이야기
      3. 또한 발생한 오류에 대하여 세부적인 메세지를 전달해야 한다.
      4. 파일에서 발생하면 FileIOException, DB라면 DatabaseException등 세부 오류를 발생해야 한다.

## 성능에 대한 이해
Web Application Server - Database 에서 가장 큰 영향을 주는 부분은 Database이다.  

가장 큰 자원을 소모하는 부분은 Database Connection Open/Close이고 한번 Open된 시점에 가능한한 많은 일을 수행 하는게 좋다.  

과거에는 Procedure로 모든 작업을 진행했지만, 현대에는 주로 ORM을 사용하는 추세이다.

과거 방식이 나쁘다기 보다는 현대에 빠른 개발을 위해서 성능을 다소 포기하는 부분일 것이고 비즈니스가 빠른 속도로 진행 되어야 하기 때문일 것이다.

주로 추천되는 방법은 ORM, 또는 CodeFirst로 빠르게 개발한 뒤 성능 이슈가 발생하는 지점을 Procedure화 하는게 추천할만 하다.

가끔 DBA들을 보면 무조건적인 Procedure 신봉자들이 있는데, 딱 한가지만 비판하고 싶다.

개발에 대한 이해가 정말 필요하다. Procedure로 작업되는 순간 모든 Application 코드 베이스가 Database에 종속되게 된다.

SI는 문제가 없겠으나 솔루션이나 서비스부분의 DBA들은 좀 더 Web Server개발에 대해 이해해야 하는 것 아닌가 한다.

또, Database 테이블 생성시 꼭 Key로 모든게 조회 되도록 해야 하고 정규화를 꼭 신경써야 한다.

Database를 무시하면 안된다. 백앤드 개발자는 꼭 DB에 대한 기본 소양은 있어야 한다. 

## 개발툴에 대한 비교
* Visual Studio 2022 : 대규모 프로젝트에 비추천. 솔루션 메모리 로딩 및 Intellisence 수집등의 성능이 좋지 못하고 19버전에 비해서 많이 좋아 졌지만 VS전용 프로젝트 개발이 아니라면 다른툴(jetbrains rider) 추천함.
* VS Code : 편집 및 디버깅 이외에 의의는 없음. 웹 개발, 소규모에 추천.
* JetBrains Rider : VS에 의존하는 프로젝트(Winform, WPF, MAUI)가 아니라면 매우 추천. 로딩 및 Intellisence 기능 매우 훌륭함.  VS와 키맵핑 호환도 가능하고 대규모 프로젝트라면 무조건 1순위 툴임.
* Mono Develop : 쓰지말자. 
* 결론 : 유료가 괜히 유료가 아니다. (Rider >> VS2022 >>>>>>> vscode)

## Acknowledgements

[JetBrains](https://www.jetbrains.com/?from=jiu-asp.net-web-solutions) kindly provides `jiu-asp.net-web-solutions` with a free open-source licence for their Resharper and Rider.
- **Resharper** makes Visual Studio a much better IDE
- **Rider** is fast & powerful cross platform .NET IDE

![image](https://upload.wikimedia.org/wikipedia/commons/thumb/1/1a/JetBrains_Logo_2016.svg/121px-JetBrains_Logo_2016.svg.png)
