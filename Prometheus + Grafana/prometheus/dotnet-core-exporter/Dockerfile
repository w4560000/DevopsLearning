FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
ARG ProjectName
WORKDIR /app

COPY $ProjectName/*.csproj ./$ProjectName/

WORKDIR /app/$ProjectName
RUN dotnet restore 

WORKDIR /app
COPY $ProjectName/. ./$ProjectName/

WORKDIR /app/$ProjectName/

RUN dotnet publish -c Release -o /app/$ProjectName/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime
ARG ProjectName

WORKDIR /app

COPY --from=build /app/$ProjectName/publish .

ENV ProjectDll="${ProjectName}.dll"
ENTRYPOINT dotnet $ProjectDll