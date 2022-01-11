FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/CodeManagerAgent/CodeManagerAgent.csproj", "CodeManagerAgent/"]
RUN dotnet restore "src/CodeManagerAgent/CodeManagerAgent.csproj"
COPY . .
WORKDIR "/src/CodeManagerAgent"
RUN dotnet build "CodeManagerAgent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeManagerAgent.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeManagerAgent.dll"]
