[dotnet PGO]  

[pgo 설명1](https://znlive.com/how-to-use-pgo-to-improve-the-performance+&cd=8&hl=ko&ct=clnk&gl=kr)  
[pgo 설명2](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-6/)
1. dotnet6 부터 도입된 프로파일 가이드 최적화
2. 런타임 중 실행 중인 함수를 분석하여 최적화하는 기술
3. 프로그램 시작시 계층화되지 않은 Tier0 코드를 빠르게 생성.
이후 다중 호출된 메소드를 다시 동적 jit 컴파일을 진행, 프로그램 실행 효율성을 향상 시킨 Tier1 코드를 생성함.
   (루프 최적화를 의미함)
4. 서버 프로그램에 사용되는 것을 권장, 클라이언트측은 아님.
5. 왜 AOT에 집착하지 않는지 알 것 같다.
6. 프로그램 크기와 실제 최적화, JIT 등의 요소를 조합해서 최적을 찾으려는 시도 같은데...