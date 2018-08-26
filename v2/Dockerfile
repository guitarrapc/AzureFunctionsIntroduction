FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/**/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY src/. ./
RUN dotnet publish -c Release -o out