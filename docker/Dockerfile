# DevFlow CLI Dockerfile
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Install Git (required for Git workflow features)
RUN apt-get update && apt-get install -y git && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file and restore dependencies
COPY ["src/DevFlow.CLI/DevFlow.CLI.csproj", "src/DevFlow.CLI/"]
RUN dotnet restore "src/DevFlow.CLI/DevFlow.CLI.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/src/DevFlow.CLI"
RUN dotnet build "DevFlow.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DevFlow.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create alias for easier usage
RUN echo '#!/bin/bash\ndotnet DevFlow.CLI.dll "$@"' > /usr/local/bin/devflow && \
    chmod +x /usr/local/bin/devflow

# Set environment variables
ENV PATH="/usr/local/bin:${PATH}"

ENTRYPOINT ["devflow"]
