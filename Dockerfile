# Giai đoạn base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

# Giai đoạn build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Sao chép toàn bộ mã nguồn vào container
COPY . .

# Di chuyển vào thư mục chứa .csproj (vì file nằm trong QLSV_API)
WORKDIR /src/QLSV_API

# Build và publish
RUN dotnet publish "QLSV_API.csproj" -c Release -o /app/out

# Giai đoạn final
FROM base AS final
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "QLSV_API.dll"]
