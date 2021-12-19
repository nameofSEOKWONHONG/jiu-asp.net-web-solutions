[캐시에 대한 설명]
##ICacheProvider
`ICacheProvider`는 캐시 제공자의 공통 인터페이스 입니다.  
각 구현체는 아래와 같습니다.
###MemoryCacheProvider
기본적인 메모리 캐시 제공자 입니다.  
캐시에 필요한 모든 구현을 제공합니다.
###LiteDbCacheProvider
`LiteDB`를 이용한 캐시 제공자 입니다.  
`LiteDB`는 로컬 파일 DB 및 Nosql database입니다.  
일부 기능은 제공하지 않습니다.  
사용전 소스를 꼭 확인하세요.  
`LiteDB`의 자세한 사항은 아래 링크를 확인하세요.  
[LiteDB](https://github.com/mbdavid/litedb)
###DistributeCacheProvider
`ASP.NET CORE`에서 지원하는 분산 캐시 제공자 입니다.  
지원하는 플랫폼은 아래와 같습니다.  
1.REDIS (사용하기전 Redis설치 및 설정 정보 학인이 필요합니다.)  
2.SQL-SERVER (사용하기전 SQL-SERVER설치 및 설정 정보 학인이 필요합니다.)
3.NCACHE  (사용하기전 NCACHE설치 및 설정 정보 학인이 필요합니다.)  
자세한 사항은 아래 링크를 확인하세요.  
[Distribute Cache](https://docs.microsoft.com/ko-kr/aspnet/core/performance/caching/distributed?view=aspnetcore-6.0)
###MSFasterCacheProvider
`Faster`는 `Microsoft`에서 만든 고성능, 대규모 KV 솔루션 입니다.
일부 기능은 제공하지 않습니다.  
사용전 소스를 꼭 확인 하세요.  
자세한 사항은 아래 링크를 확인 하세요.  
[Faster](https://github.com/microsoft/FASTER)