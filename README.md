# GamxorOila / Family Care — Backend (C# .NET 9, Clean Architecture)

[gamxor_oila_flutter_version](https://github.com/fismoilova01/gamxor_oila_flutter_version)
Flutter ilovasi uchun backend. Mijozning mavjud API kontrakti (endpointlar, JSON
maydonlari, enum qiymatlari) to'liq saqlangan holda qayta yozilgan — Flutter kodida
hech narsa o'zgartirilmaydi, faqat ilovadagi **server manzili** yangi backendga
qaratiladi.

## Arxitektura (Clean Architecture)

```
src/
  GamxorOila.Domain          # Entitilar, enumlar, value object'lar (hech narsaga bog'liq emas)
  GamxorOila.Application     # Use-case'lar, DTO'lar, interfeyslar, mapping (Domain ga bog'liq)
  GamxorOila.Infrastructure  # EF Core (PostgreSQL), repozitoriy, fayl saqlash, soat, OTP
  GamxorOila.Api             # ASP.NET Core controllerlar, DI, middleware
tests/
  GamxorOila.IntegrationTests # WebApplicationFactory + SQLite in-memory orqali to'liq stack testlari
```

Bog'liqlik yo'nalishi: `Api → Infrastructure → Application → Domain`.
Domain va Application qatlamlari hech qanday infratuzilma (DB, web) detallarini bilmaydi.

## Texnologiyalar

- .NET 9 / ASP.NET Core
- Entity Framework Core 9 + **PostgreSQL** (Npgsql)
- Docker (server tanlamaydigan, istalgan joyga deploy bo'ladi)

## Production xususiyatlari

- **Serilog** — strukturali logging + so'rov loglari
- **Global xato-handler** — kutilmagan xatolar uchun toza `ProblemDetails` JSON
- **Health-check'lar** — `/health` (liveness), `/health/ready` (baza ulanishini tekshiradi)
- **Rate limiting** — OTP so'roviga (`auth/request-code`) IP bo'yicha 5 so'rov/daqiqa
- **Validatsiya** — FluentValidation; xato bo'lsa mijoz kutadigan `{ success:false, message }` qaytaradi (HTTP 400 emas)
- **Xavfsizlik** — `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy` header'lari; sozlanadigan CORS (`Cors:AllowedOrigins`)
- **Reverse-proxy IP** — `X-Forwarded-For` orqali haqiqiy mijoz IP si (Render va h.k.)

## API kontrakti

Barcha endpointlar `/api/` ostida, `POST` (mijoz trailing slash bilan yuboradi —
middleware uni normallashtiradi). Autentifikatsiya `deviceId` orqali (qurilma = hisob).

| Endpoint | Tavsif |
|---|---|
| `POST /api/bootstrap/` | Ilova holatini qaytaradi (`{ state }`) |
| `POST /api/auth/request-code/` | SMS kod so'rovi (demo kodi `otpHint`da) |
| `POST /api/auth/verify-code/` | Kodni tasdiqlash |
| `POST /api/auth/register/` | Ro'yxatdan o'tishni yakunlash |
| `POST /api/profile/save/` | Profilni saqlash |
| `POST /api/members/select/` | A'zoni tanlash |
| `POST /api/refresh/` | Ma'lumotlarni yangilash |
| `POST /api/invitations/send/` | Taklif yuborish |
| `POST /api/invitations/{id}/accept/` | Taklifni qabul qilish |
| `POST /api/notifications/{id}/read/` | O'qildi deb belgilash |
| `POST /api/notifications/{id}/dismiss/` | Bildirishnomani o'chirish |
| `POST /api/notifications/read-all/` | Barchasini o'qildi qilish |
| `POST /api/sos/trigger/` | SOS yuborish |
| `POST /api/sos/clear/` | SOS to'xtatish |
| `POST /api/sign-out/` | Tizimdan chiqish |
| `POST /api/uploads/assets/` | Fayl (avatar) yuklash — multipart |
| `GET /health` | Health-check |

Boshqa javoblar `{ success, message, state? }` ko'rinishida.

## Mahalliy ishga tushirish

### 1-variant: Docker Compose (tavsiya, hammasi birga)

```bash
cd backend
docker compose up --build
```

API: `http://localhost:8080`, Swagger: `http://localhost:8080/swagger`.
PostgreSQL avtomatik ko'tariladi va migratsiyalar startda qo'llanadi.

### 2-variant: Lokal .NET + lokal/Docker Postgres

```bash
# Postgres (Docker bilan)
docker run -d --name gamxor-pg -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=gamxor -p 5432:5432 postgres:16

cd backend
dotnet run --project src/GamxorOila.Api
```

Ulanish satri `appsettings.Development.json` da (`Host=localhost`). Startda migratsiyalar
avtomatik qo'llanadi.

## Testlar

```bash
cd backend
dotnet test
```

Testlar haqiqiy Postgres talab qilmaydi (SQLite in-memory orqali butun HTTP stack'ni
tekshiradi: bootstrap seeding, auth oqimi, SOS, takliflar, bildirishnomalar, enum
qiymatlari, camelCase serializatsiya, trailing-slash routing).

## Konfiguratsiya (muhit o'zgaruvchilari)

Backend muayyan hostingga bog'lanmaydi — ulanishni quyidagilardan oladi:

| O'zgaruvchi | Tavsif |
|---|---|
| `DATABASE_URL` | `postgres://user:pass@host:port/db` (Render, Railway, Heroku, Fly.io, Neon, Supabase) |
| `ConnectionStrings__Postgres` | To'liq Npgsql ulanish satri (alternativa) |
| `PORT` | Tinglash porti (hostlar avtomatik beradi; bo'lmasa 8080) |
| `Otp__FixedCode` | Demo SMS kodi (default `2580`). Bo'sh qilsangiz — tasodifiy 4 xonali kod |
| `FileStorage__RootPath` | Yuklangan fayllar papkasi (default `wwwroot/media`) |
| `EnableSwagger` | Productionda Swagger'ni yoqish (`true`/`false`) |

## Globalga deploy qilish (server-agnostik)

Backend Docker image sifatida qadoqlangan, shuning uchun **istalgan** platformaga
chiqadi. Asosiy talab: ishlovchi PostgreSQL va `DATABASE_URL` (yoki
`ConnectionStrings__Postgres`).

### Render.com
1. Yangi **Web Service** → "Build from a Dockerfile", repo'ni ulang, root = `backend`.
2. Yangi **PostgreSQL** instance yarating; uning Internal `DATABASE_URL` ini
   Web Service environment'iga qo'shing.
3. Deploy. Render `PORT` ni avtomatik beradi.

### Railway / Fly.io / Heroku
- Postgres plagin/addon qo'shing → `DATABASE_URL` avtomatik in'ektsiya qilinadi.
- Dockerfile aniqlanadi; `PORT` platforma tomonidan beriladi.
- Fly.io uchun: `fly launch` (Dockerfile'ni topadi) + `fly postgres create` + `fly postgres attach`.

### O'zingizning VPS / istalgan bulut (Docker)
```bash
docker build -t gamxor-api ./backend
docker run -d -p 80:8080 \
  -e "ConnectionStrings__Postgres=Host=DB_HOST;Database=gamxor;Username=...;Password=..." \
  gamxor-api
```
Old tomonda HTTPS uchun Nginx/Caddy/Traefik reverse-proxy qo'ying.

## Flutter ilovasini ulash

Deploydan so'ng ilovada server manzilini yangilang (profil → server sozlamalari):
```
https://SIZNING-DOMEN/api/
```
Standart qiymat (`lib/data/repository.dart`) Android emulyator uchun
`http://10.0.2.2:8000/api/` — bu faqat lokal test uchun.
