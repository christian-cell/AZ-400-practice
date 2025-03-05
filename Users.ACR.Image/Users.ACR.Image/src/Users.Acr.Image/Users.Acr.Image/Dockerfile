# Imagen base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Users.Acr.Image.csproj", "./"]
RUN dotnet restore "Users.Acr.Image.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Users.Acr.Image.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publicación
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN rm /src/Users.Acr.Image.deps.json || true
RUN rm /src/Users.Acr.Image.runtimeconfig.json || true
RUN dotnet publish "Users.Acr.Image.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.Acr.Image.dll"]