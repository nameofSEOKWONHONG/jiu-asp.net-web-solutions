FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/99.SampleApps/ZstdNetSample/ZstdNetSample.csproj", "ZstdNetSample/"]
RUN dotnet restore "src/99.SampleApps/ZstdNetSample/ZstdNetSample.csproj"
COPY . .
WORKDIR "/src/ZstdNetSample"
RUN dotnet build "ZstdNetSample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZstdNetSample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZstdNetSample.dll"]
