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
COPY ["Mikroservis.Email/Mikroservis.Email/Mikroservis.Email.csproj", "Mikroservis.Email/"]
RUN dotnet restore "./Mikroservis.Email/Mikroservis.Email.csproj"

# Copy the rest of the files and build the project
COPY . . 
WORKDIR "/src/Mikroservis.Email/Mikroservis.Email"
RUN dotnet build "./Mikroservis.Email.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Mikroservis.Email.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image for running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mikroservis.Email.dll"]
