# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY CVBackend/CVBackend.csproj CVBackend/
COPY CVBackend.Core/CVBackend.Core.csproj CVBackend.Core/
COPY CVBackend.Shared/CVBackend.Shared.csproj CVBackend.Shared/
RUN dotnet restore CVBackend/CVBackend.csproj

# Copy all source files
COPY CVBackend/ CVBackend/
COPY CVBackend.Core/ CVBackend.Core/
COPY CVBackend.Shared/ CVBackend.Shared/

# Build the project
RUN dotnet build CVBackend/CVBackend.csproj -c Release -o /app/build

# Publish the project
RUN dotnet publish CVBackend/CVBackend.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

# Expose ports (5000 for local, 8080 for Render)
EXPOSE 5000 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application as non-root user
USER $APP_UID
ENTRYPOINT ["dotnet", "CVBackend.dll"]
