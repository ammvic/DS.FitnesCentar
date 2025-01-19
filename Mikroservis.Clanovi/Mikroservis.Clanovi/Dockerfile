# Use base image from .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use SDK image for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the project file to optimize Docker caching
COPY ["Mikroservis.Clanovi/Mikroservis.Clanovi/Mikroservis.Clanovi.csproj", "Mikroservis.Clanovi/"]
RUN dotnet restore "./Mikroservis.Clanovi/Mikroservis.Clanovi.csproj"

# Copy the rest of the files and build the project
COPY . . 
WORKDIR "/src/Mikroservis.Clanovi/Mikroservis.Clanovi"
RUN dotnet build "./Mikroservis.Clanovi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Mikroservis.Clanovi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image for running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mikroservis.Clanovi.dll"]