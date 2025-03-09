# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY . ./
RUN dotnet restore

# Build and publish the project in Release configuration
RUN dotnet publish -c Release -o out

# Runtime stage using ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/out .

# Run the console application
ENTRYPOINT ["dotnet", "ConsoleApp1.dll"]
