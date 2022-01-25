FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/CodeManagerAgentManager/CodeManagerAgentManager.csproj", "CodeManagerAgentManager/"]
RUN dotnet restore "src/CodeManagerAgentManager/CodeManagerAgentManager.csproj"
COPY . .
WORKDIR "/src/CodeManagerAgentManager"
RUN dotnet build "CodeManagerAgentManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeManagerAgentManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeManagerAgentManager.dll"]
