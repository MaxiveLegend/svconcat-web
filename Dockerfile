# syntax=docker/dockerfile:1

# ---------------------------------------------------------------------------
# Stage 1: Build the Vite frontend assets.
# Vite is configured (frontend/vite.config.js) to emit into
# ../SvConcatWeb/wwwroot/dist, so we keep the repo's relative folder layout
# (frontend/ next to SvConcatWeb/) inside this stage.
# ---------------------------------------------------------------------------
FROM node:22-bookworm-slim AS frontend
WORKDIR /src/frontend

# Restore deps first for better layer caching.
COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci

# Copy the rest of the frontend source and build the production assets.
COPY frontend/ ./
RUN npm run prod
# Built assets now live at /src/SvConcatWeb/wwwroot/dist

# ---------------------------------------------------------------------------
# Stage 2: Restore + publish the .NET 10 Umbraco app.
# ---------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore using only the csproj for better layer caching.
COPY SvConcatWeb/SvConcatWeb.csproj SvConcatWeb/
RUN dotnet restore SvConcatWeb/SvConcatWeb.csproj

# Copy the application source.
COPY SvConcatWeb/ SvConcatWeb/

# Bring in the compiled frontend assets from the frontend stage.
COPY --from=frontend /src/SvConcatWeb/wwwroot/dist SvConcatWeb/wwwroot/dist

RUN dotnet publish SvConcatWeb/SvConcatWeb.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ---------------------------------------------------------------------------
# Stage 3: Runtime image.
# Debian-based aspnet image (not alpine) so the app-local ICU package and
# globalization behave consistently.
# ---------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# curl is used by the container HEALTHCHECK (see docker-compose.yml).
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build /app/publish ./

# Persisted at runtime via volumes (see docker-compose.yml):
#   - umbraco/Data  -> SQLite database, Examine indexes, NuCache, TEMP
#   - wwwroot/media -> uploaded media
RUN mkdir -p /app/umbraco/Data /app/wwwroot/media

EXPOSE 8080

ENTRYPOINT ["dotnet", "SvConcatWeb.dll"]
