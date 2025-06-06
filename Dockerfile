# Dockerfile đề xuất
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Sao chép toàn bộ mã nguồn
COPY . .

# Build đúng file .csproj ở thư mục gốc
RUN dotnet publish "./QLSV_API.csproj" -c Release -o /app/out

FROM base AS final
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "QLSV_API.dll"]
