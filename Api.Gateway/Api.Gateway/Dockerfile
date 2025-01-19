# Base image with .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image with .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the project file for better caching
COPY ["Api.Gateway/Api.Gateway/Api.Gateway.csproj", "Api.Gateway/"]
RUN dotnet restore "./Api.Gateway/Api.Gateway.csproj"

# Copy the remaining files and build the project
COPY . . 
WORKDIR "/src/Api.Gateway/Api.Gateway"
RUN dotnet build "./Api.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Api.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.Gateway.dll"]
