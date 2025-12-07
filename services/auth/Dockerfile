# Use the official .NET 9 runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7002

# Use the official .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["TouristAgencyAuth/TouristAgencyAuth/TouristAgencyAuth.Docker.csproj", "TouristAgencyAuth/TouristAgencyAuth.csproj"]
COPY ["TouristAgencyShared/TouristAgencyShared/TouristAgencyShared.csproj", "TouristAgencyShared/"]

# Restore dependencies
RUN dotnet restore "TouristAgencyAuth/TouristAgencyAuth.csproj"

# Copy the rest of the source code
COPY . .

# Publish the application
WORKDIR "/src/TouristAgencyAuth"
RUN dotnet publish "TouristAgencyAuth.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:7002
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TouristAgencyAuth.dll"]