## 운영 계획
1. Database에서 생성할 엑셀파일 내역을 추출하여 서버 스토리지에 파일 작성(예:txt, csv)
2. 스케줄러가 파일을 읽어 데이터 생성 및 엑셀 출력.
3. 출력된 엑셀 파일 및 첨부파일을 압축하여 스토리지에 업로드
4. 압축된 첨부파일을 메일 발송.
5. 압축파일 및 생성 파일들 삭제.
   * 각 파일 생성은 독자 디렉토리 생성하여 진행
   * 작업 완료된 경우 일괄 삭제
   * hangfire나 qutarz 사용하여 스케줄러로 1차 구현. (서버측에서)
   * 이후 worker 서버 별도 구성할 수 있을 경우 kafka등을 이용한다.
   * 마감 이후에 재정산 또는 재다운로드 할 경우 유료 서비스로 제공할지...(횟수는 10회 까지만)
   * 보관 기간은 5년

## 구현비교
1. 구현해본바, ClosedXml과 FastExcel은 약 9~10배 성능차이.
2. 파일을 직접 생성하는 것과 읽어들인 Template파일에서 생성하는 것과 차이가 있는 것으로 생각됨.
3. DataTable, DataSet을 사용하는 것보다 직접 맵핑하는게 더 빠르다.
4. FastExcel의 치명적 문제는 서식 적용이 안됨. 원본에 서식이 적용되더라도 최종결과물에서 서식 적용이 안됨.
5. 빠른 결과 만을 원한다면 FastExcel, 각종 서식과 Excel 지원을 필요로 한다면 ClosedXml, Native에 가깝게 개발하고 싶다면 OpenXmlSdk를 사용하자.
6. ClosedXml은 OpenXmlSdk Wrapper이다.

## 구현전략
1. ClosedXml에서 Excel작성시 FastExcel과 같은 방법을 취한다.
2. 출력물에 대한 모든 템플릿을 MemoryStream 또는 파일을 복사해서 생성하는 방식으로로 구현함.
   1. 구현해본 결과 성능향상은 미미함.
   2. 대량 데이터에서 성능향상 있을 수 있음.
   3. 단, 스타일링 필요없는 대량 데이터의 경우라면 fastxml을 사용하는게 더 나아보임.

## ClosedXml(C#) vs POI(JAVA) - 300Row, 25Column, Value 동일 조건.
| 회차  |ClosedXml|POI|
|-----|---------|---|
| 1   |0.6610683|0.490395|
| 2   |0.0929961|0.1592382|
| 3   |0.0851467|0.1348167|
| 4   |0.0851467|0.1348167|
| 5   |0.0690752|0.1239471|
| 6   |0.0665898|0.122226|
| 7   |0.0642861|0.1189653|
| 8   |0.0670824|0.1202132|
| 9   |0.0615653|0.1167813|
| 10  |0.0668502|0.1192532|
`약 2배 성능차이, 초기 실행시간은 ClosedXml이 다소 느림.`

* Benchmark 500 ROW, 25 COL     
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.978/21H2)  
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores  
.NET SDK=7.0.100-rc.1.22431.12  
[Host]     : .NET 7.0.0 (7.0.22.42610), X64 RyuJIT AVX2  
DefaultJob : .NET 7.0.0 (7.0.22.42610), X64 RyuJIT AVX2  


| Method |     Mean |    Error |   StdDev | Completed Work Items | Lock Contentions |      Gen0 |      Gen1 |     Gen2 | Allocated |
|------- |---------:|---------:|---------:|---------------------:|-----------------:|----------:|----------:|---------:|----------:|
| Runner | 76.43 ms | 1.416 ms | 2.796 ms |                    - |                - | 4500.0000 | 2333.3333 | 500.0000 |  33.94 MB |
