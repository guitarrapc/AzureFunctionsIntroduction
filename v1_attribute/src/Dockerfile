FROM microsoft/dotnet-framework:4.7.2-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY AzureFunctionsIntroduction/*.csproj ./AzureFunctionsIntroduction/
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /app/AzureFunctionsIntroduction
RUN dotnet build

FROM build AS publish
WORKDIR /app/AzureFunctionsIntroduction
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet-framework:4.7.2-runtime AS runtime
WORKDIR /app
COPY --from=publish /app/AzureFunctionsIntroduction/out ./
ENTRYPOINT ["powershell.exe"]