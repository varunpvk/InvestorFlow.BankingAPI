# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["IF.BankingAPI.csproj", "."]
RUN dotnet restore "./IF.BankingAPI.csproj"

# Copy the rest of the application code
COPY . .
RUN dotnet build "./IF.BankingAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "./IF.BankingAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Define the entry point for the container
ENTRYPOINT ["dotnet", "IF.BankingAPI.dll"]