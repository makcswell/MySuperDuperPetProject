FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

ENV ASPNETCORE_URLS=http://*:3000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UsersService/UsersService.csproj", "UsersService/"]
COPY ["StaticStorages/StaticStorages.csproj", "StaticStorages/"]
COPY ["CustomExtensions/CustomExtensions.csproj", "CustomExtensions/"]
COPY ["MyDatabase/MyDatabase.csproj", "MyDatabase/"]
COPY ["Middlewares/Middlewares.csproj", "Middlewares/"]
RUN dotnet restore "UsersService/UsersService.csproj"
COPY . .
WORKDIR "/src/UsersService"
RUN dotnet build "UsersService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UsersService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.dll"]
