# Giai đoạn base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Giai đoạn build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .
WORKDIR /src/QLSV_API

RUN dotnet publish "QLSV_API.csproj" -c Release -o /app/out

# Giai đoạn final
FROM base AS final
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "QLSV_API.dll"]
