# ── Stage 1: Build ────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj dan restore dependency lebih dulu (layer caching)
COPY ["MailDesk.API/MailDesk.API.csproj", "MailDesk.API/"]
RUN dotnet restore "MailDesk.API/MailDesk.API.csproj"

# Copy seluruh source code lalu build
COPY . .
WORKDIR /src/MailDesk.API
RUN dotnet build "MailDesk.API.csproj" -c Release -o /app/build

# ── Stage 2: Publish ──────────────────────────────────────────
FROM build AS publish
RUN dotnet publish "MailDesk.API.csproj" -c Release -o /app/publish \
    --no-restore

# ── Stage 3: Runtime (image final, sekecil mungkin) ──────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Salin hasil publish dari stage sebelumnya
COPY --from=publish /app/publish .

# Expose port HTTP
EXPOSE 5000

# Jalankan aplikasi
ENTRYPOINT ["dotnet", "MailDesk.API.dll"]