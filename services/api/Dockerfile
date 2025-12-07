# Use the official .NET 9.0 runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
COPY ["services/api/TouristAgencyAPI.Docker.csproj", "services/api/TouristAgencyAPI.csproj"]
COPY ["services/shared/TouristAgencyShared.csproj", "services/shared/"]
RUN dotnet restore "services/api/TouristAgencyAPI.csproj"

# Copy the rest of the source code
COPY . .

# Publish the application
WORKDIR "/src/services/api"
RUN dotnet publish "TouristAgencyAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "TouristAgencyAPI.dll"] 