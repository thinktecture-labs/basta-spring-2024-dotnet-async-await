FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.19 as base
RUN apk add --no-cache icu-libs
WORKDIR /app
RUN adduser -u 1000 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine3.19 as build
WORKDIR /src
COPY ["./AsyncVsSync.App.csproj", "./AsyncVsSync.App.csproj"]
RUN dotnet restore "./AsyncVsSync.App.csproj"
COPY . .
RUN dotnet publish "./AsyncVsSync.App.csproj" \
    -c Release \
    --no-restore \
    -o "../app"

FROM base
WORKDIR /app
COPY --chown=appuser --from=build /app .
