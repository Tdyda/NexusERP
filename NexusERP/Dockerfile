# Etap 1: Budowanie aplikacji z .NET SDK 9.0
FROM mcr.microsoft.com/dotnet/sdk:9.0-windowsservercore-ltsc2022 AS build

# Ustawienie katalogu roboczego
WORKDIR /source

# Kopiujemy lokalny plik instalatora SDK (jeśli masz taki plik, jeśli nie, możesz usunąć tę linię)
COPY DockerFiles/dotnet-sdk-9.0.200-win-x64.exe dotnet-sdk.exe

# Instalacja .NET SDK
RUN dotnet-sdk.exe /quiet /norestart && del dotnet-sdk.exe

# Kopiujemy kod źródłowy aplikacji
COPY . .

# Przywracamy zależności i kompilujemy aplikację
RUN dotnet restore
RUN dotnet publish -c Release -r win-x64 -o /app

# Etap 2: Uruchamianie aplikacji w osobnym kontenerze z .NET Runtime 9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0-windowsservercore-ltsc2022

# Ustawienie katalogu roboczego
WORKDIR /app

# Kopiujemy lokalny plik instalatora Runtime (jeśli masz taki plik, jeśli nie, możesz usunąć tę linię)
COPY DockerFiles/aspnetcore-runtime-9.0.2-win-x64.exe dotnet-runtime.exe

# Instalacja .NET Runtime
RUN dotnet-runtime.exe /quiet /norestart && del dotnet-runtime.exe

# Kopiujemy skompilowaną aplikację z etapu budowy
COPY --from=build /app .

# Uruchamiamy aplikację
CMD ["dotnet", "/app/NexusERP.dll"]
