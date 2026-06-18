# ---- Build bosqichi ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore'ni keshlash uchun avval faqat loyiha fayllarini ko'chiramiz.
COPY GamxorOila.sln ./
COPY src/GamxorOila.Domain/*.csproj src/GamxorOila.Domain/
COPY src/GamxorOila.Application/*.csproj src/GamxorOila.Application/
COPY src/GamxorOila.Infrastructure/*.csproj src/GamxorOila.Infrastructure/
COPY src/GamxorOila.Api/*.csproj src/GamxorOila.Api/
RUN dotnet restore src/GamxorOila.Api/GamxorOila.Api.csproj

# Qolgan manbalarni ko'chiramiz va nashr qilamiz.
COPY . .
RUN dotnet publish src/GamxorOila.Api/GamxorOila.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime bosqichi ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./

# Konteyner ichida 8080-portda tinglaymiz (PORT env mavjud bo'lsa, dastur uni ishlatadi).
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GamxorOila.Api.dll"]
