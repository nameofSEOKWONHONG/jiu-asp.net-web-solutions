# jiu-asp.net-web-solutions

`jiu-asp.net-web-solutions`는 asp.net core 6기준으로 제작하는 서버 및 Blazor Server / WASM 프로젝트 템플릿입니다.

이 프로젝트의 "https://github.com/blazorhero/CleanArchitecture"에 기초하고 있습니다.

프로젝트의 구조는 아래와 같습니다.

1. Share  : 공유 가능한 클래스 라이브러리 집합니다.
    - Core : 프로젝트의 핵심 코드 집합입니다.
        - Application
            - Application의 기본 코드 집합니다.
            - 추상적 Base Class 및 기반 인프라코드를 정의합니다.
            - 제작될 모든 Application Project에서 사용합니다.
        - Domain
            - Entity, Dto, Enum등의 데이터 기반 코드 집합니다.
            - Database정의 및 전달 Parameter등을 정의 합니다.
        - eXtensionSharp
            - static class library의 집합입니다.
        - Infrastructure
            - Web Application에 사용될 인프라코드의 집합입니다.
            - `Application`은 `Infrastrcture`를 참조할 수 없습니다.
            - Web Application에 사용될 코드만을 정의 합니다.

    - Project : Application을 정의합니다.
        - 실제 구현할 Application 코드를 정의합니다.
        - Service 및 MediatR등으로 구성됩니다.
        
2. Server : 서버 Application 입니다.
    - Project : `1.Share>Project`를 참조하여 만드는 개별 Web Api Project 입니다.  
    MSA 형식의 구성을 갖을 수 있습니다.
    - WebApiApplication : 모놀리식으로 구성되는 Web Api Application 입니다.
3. Client : Client Application 입니다. Blazor Server, Blazor Wasm 프로젝트 입니다.
    - Share
        - Blazor Server와 Blazor Wasm에서 모두 사용할 클라이언트 비즈니스 로직을 정의합니다.
        - 해당 프로젝트에서는 `1.Share>Core>Infrastructure`를 사용할 수 없습니다.
    - BlazorServerApplication : Blazor Server Application 입니다.
    - BlazorWasmApplication : Blazor Wasm Application 입니다.

** 중요사항 **
1. `1.Share>Core>Infrastructure`는 Web Api Server Application을 위한 구성입니다.  
따라서 해당 프로젝트는 Web Api Server Application에서만 사용합니다.
2. `Interface`의 정의는 `1.Share>Core>Application>Interfaces`에 작성합니다.  
실제 구현은 각각의 Application 프로젝트에서 담당합니다.
3. Web Api Application에서 Blazor WASM을 호스팅 할 수 있습니다.  
관련 코드는 `2.Server>WebApiApplication>Startup.cs`를 참고 합니다.