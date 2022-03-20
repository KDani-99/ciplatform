FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY "src/CIPlatform.Core" "CIPlatform.Core"
COPY "src/CIPlatform.Data" "CIPlatform.Data"
COPY "src/CIPlatformWebApi/CIPlatformWebApi.csproj" "CIPlatformWebApi/"
COPY "src/CIPlatformWebApi" "CIPlatformWebApi/"
RUN dotnet restore "CIPlatformWebApi/CIPlatformWebApi.csproj"
WORKDIR "/src/CIPlatformWebApi"
RUN dotnet build "CIPlatformWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CIPlatformWebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CIPlatformWebApi.dll"]
