# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore

# Build the specific API project
# We move into the API folder to build it specifically
WORKDIR "/src/InsuranceManagement.API"
RUN dotnet publish -c Release -o /app

# Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Check your project name! 
# If your project folder is InsuranceManagement.API, 
# the DLL is likely named InsuranceManagement.API.dll
ENTRYPOINT ["dotnet", "InsuranceManagement.API.dll"]
