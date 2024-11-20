# syntax=docker/dockerfile:1

FROM node:18.16.1-alpine3.18 as frontend-env
WORKDIR /App
COPY frontend/. .
RUN --mount=type=cache,target=/root/.yarn YARN_CACHE_FOLDER=/root/.yarn yarn 
RUN --mount=type=cache,target=/root/.yarn YARN_CACHE_FOLDER=/root/.yarn yarn build

FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build-env
WORKDIR /App
COPY ./backend/. ./
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages dotnet build -c Release -o out
RUN DEBIAN_FRONTEND=noninteractive TZ=Asia/Jakarta apt-get -y install tzdata

# Stage to install dependencies (pdftk and ImageMagick) with APT cache
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy as runtime-env
WORKDIR /App

# Use APT cache to speed up dependency installations
RUN apt-get update && \
    --mount=type=cache,target=/var/cache/apt --mount=type=cache,target=/var/lib/apt \
    DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
    pdftk imagemagick && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Final stage
FROM runtime-env as final-env
ENV TZ=Asia/Jakarta
WORKDIR /App

# Copy files from build and frontend stages
COPY --from=build-env /usr/share/zoneinfo /usr/share/zoneinfo
COPY --from=build-env /App/out .
COPY --from=frontend-env /App/dist/. ./wwwroot

# Create the directory for file uploads and set permissions
RUN mkdir -p /App/files && \
    chown -R app:app /App/files && \
    chmod 777 /App/files

# ENTRYPOINT ["tail", "-f", "/dev/null"]
ENTRYPOINT ["dotnet", "backend.dll"]
