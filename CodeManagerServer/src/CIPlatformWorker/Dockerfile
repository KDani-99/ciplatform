FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/CIPlatformWorker/CIPlatformWorker.csproj", "CIPlatformWorker/"]
RUN dotnet restore "src/CIPlatformWorker/CIPlatformWorker.csproj"
COPY . .
WORKDIR "/src/CIPlatformWorker"
RUN dotnet build "CIPlatformWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CIPlatformWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CIPlatformWorker.dll"]
