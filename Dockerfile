# ============================================================
# PetHealthAPI - Dockerfile Multi-Stage para .NET 8
# Challenge FIAP 2026 - DevOps Tools & Cloud Computing
# Otimizado para VMs com 1GB RAM
# ============================================================

# ------------------------------------------------------------
# STAGE 1: BUILD
# ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /src

COPY PetHealthAPI/*.csproj ./PetHealthAPI/
RUN dotnet restore PetHealthAPI/PetHealthAPI.csproj

COPY PetHealthAPI/. ./PetHealthAPI/

WORKDIR /src/PetHealthAPI
RUN dotnet publish -c Release -o /app/publish --no-restore \
    /p:PublishTrimmed=false \
    /p:UseAppHost=false

# ------------------------------------------------------------
# STAGE 2: RUNTIME — Alpine (~100MB menor que Debian)
# ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

LABEL maintainer="PetHealth Team - FIAP 2026"
LABEL description="API .NET 8 do projeto PetHealth (Challenge FIAP)"
LABEL version="1.0.0"

WORKDIR /app

# Instala curl para o healthcheck (Alpine não tem por padrão)
RUN apk add --no-cache curl

# Cria usuário não-root (exigência do Challenge)
RUN addgroup --system --gid 1001 petgroup \
 && adduser  --system --uid 1001 --ingroup petgroup --shell /bin/false petuser

COPY --from=build --chown=petuser:petgroup /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_GCConserveMemory=9

EXPOSE 8080

USER petuser

HEALTHCHECK --interval=30s --timeout=5s --start-period=60s --retries=3 \
  CMD curl -fsS http://localhost:8080/swagger/v1/swagger.json || exit 1

ENTRYPOINT ["dotnet", "PetHealthAPI.dll"]
