# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution/project files
COPY . ./

# Restore dependencies
RUN dotnet restore InsuranceManagement.API/InsuranceManagement.API.csproj

# Build & publish the API project
RUN dotnet publish InsuranceManagement.API/InsuranceManagement.API.csproj -c Release -o out

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/out .

# Set environment to Production so appsettings.Production.json is loaded
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port (Render will override via PORT env)
EXPOSE 8080

# Start app
ENTRYPOINT ["dotnet", "InsuranceManagement.API.dll"]