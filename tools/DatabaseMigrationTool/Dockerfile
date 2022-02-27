FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["tools/DatabaseMigrationTool/DatabaseMigrationTool.csproj", "DatabaseMigrationTool/"]
RUN dotnet restore "tools/DatabaseMigrationTool/DatabaseMigrationTool.csproj"
COPY . .
WORKDIR "/src/DatabaseMigrationTool"
RUN dotnet build "DatabaseMigrationTool.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatabaseMigrationTool.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatabaseMigrationTool.dll"]
