FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["MonitorAtivosB3/MonitorAtivosB3.csproj", "MonitorAtivosB3/"]
RUN dotnet restore "MonitorAtivosB3/MonitorAtivosB3.csproj"

COPY MonitorAtivosB3/ MonitorAtivosB3/
WORKDIR "/src/MonitorAtivosB3"
RUN dotnet build "./MonitorAtivosB3.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MonitorAtivosB3.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitorAtivosB3.dll"]
