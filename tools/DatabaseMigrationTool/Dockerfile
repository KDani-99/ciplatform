FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY "src/CIPlatform.Core" "CIPlatform.Core/"
COPY "src/CIPlatform.Data" "CIPlatform.Data/"
COPY "src/CIPlatformWebApi" "CIPlatformWebApi/"
COPY "src/CIPlatformWebApi" "CIPlatformWebApi/"
WORKDIR /tools
COPY "tools/DatabaseMigrationTool" "DatabaseMigrationTool/"
RUN dotnet restore "DatabaseMigrationTool/DatabaseMigrationTool.csproj"
WORKDIR /tools/DatabaseMigrationTool
RUN dotnet build "DatabaseMigrationTool.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatabaseMigrationTool.csproj" -c Release -o /app/publish
RUN dotnet tool install --global dotnet-ef
ENV PATH="/root/.dotnet/tools:${PATH}"
WORKDIR /src/CIPlatformWebApi
RUN dotnet ef migrations add CIPlatform -o /app/Migrations

FROM publish AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatabaseMigrationTool.dll"]
