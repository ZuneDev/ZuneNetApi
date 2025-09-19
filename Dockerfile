# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-shared

# Copy, restore, and build common libraries in a separate stage
# so it can be cached
WORKDIR /source
COPY ./ .
RUN dotnet restore
WORKDIR /source/Zune.Net.Shared
RUN dotnet build -c release --no-restore /p:EnableNETAnalyzers=False /p:AnalysisLevel=none /p:RunAnalyzers=false /p:UseRoslynAnalyzers=false
    
FROM build-shared AS build

# Take the target project name as an argument:
# this allows the same Dockerfile to be used for
# all Zune.Net.* microservices
ARG PROJ_NAME
ENV PROJ_NAME=${PROJ_NAME:-specifyaproject_pls}

# Publish the results
WORKDIR /source/${PROJ_NAME}
RUN dotnet publish -c release -o /app --no-restore /p:EnableNETAnalyzers=False /p:AnalysisLevel=none /p:RunAnalyzers=false /p:UseRoslynAnalyzers=false

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ARG PROJ_NAME
ENV PROJ_NAME=${PROJ_NAME}
WORKDIR /app
COPY --from=build /app .
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT dotnet ${PROJ_NAME}.dll
