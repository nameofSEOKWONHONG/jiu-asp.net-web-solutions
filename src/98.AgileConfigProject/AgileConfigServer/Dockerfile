FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/2.Server/AgileConfigServer/AgileConfigServer.csproj", "AgileConfigServer/"]
RUN dotnet restore "src/2.Server/AgileConfigServer/AgileConfigServer.csproj"
COPY . .
WORKDIR "/src/AgileConfigServer"
RUN dotnet build "AgileConfigServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgileConfigServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgileConfigServer.dll"]
