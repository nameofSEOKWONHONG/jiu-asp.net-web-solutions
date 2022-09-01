FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/99.SampleApps/OpenXmlSample/OpenXmlSample.csproj", "OpenXmlSample/"]
RUN dotnet restore "src/99.SampleApps/OpenXmlSample/OpenXmlSample.csproj"
COPY . .
WORKDIR "/src/OpenXmlSample"
RUN dotnet build "OpenXmlSample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenXmlSample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenXmlSample.dll"]
