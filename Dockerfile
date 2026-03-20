# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the entire solution and projects
COPY . .

# Restore dependencies
RUN dotnet restore "InsuranceManagement.sln"

# Move into the API folder
WORKDIR "/src/InsuranceManagement.API"

# Build and Publish with warnings suppressed
# This ignores CS8618 (nullability), CS8601/02/03 (null references), and CS8981 (naming)
RUN dotnet publish "InsuranceManagement.API.csproj" -c Release -o /app/publish \
    /p:NoWarn="CS8618;CS8601;CS8602;CS8603;CS8604;CS8613;CS8625;CS8629;CS8981" \
    /p:TreatWarningsAsErrors=false

# Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# The DLL name based on your project structure
ENTRYPOINT ["dotnet", "InsuranceManagement.API.dll"]
