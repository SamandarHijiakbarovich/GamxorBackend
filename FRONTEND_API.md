# Frontend uchun API qo'llanmasi — GamxorOila / Family Care

Backend manzili (production):

```
https://gamxor-api.onrender.com/api/
```

Interaktiv hujjat (barcha endpointlarni shu yerda sinab ko'rsa bo'ladi):
**https://gamxor-api.onrender.com/swagger**

> Bepul rejada servis faollik bo'lmasa "uxlaydi" — birinchi so'rov ~50s kutishi mumkin.

---

## 1. Asosiy qoidalar

- **Barcha endpointlar `POST`** (faqat `/health` va `/` — `GET`).
- Manzillar **trailing slash bilan** yuboriladi: `…/api/bootstrap/`.
- `Content-Type: application/json` (fayl yuklash bundan mustasno — `multipart/form-data`).
- Javoblar **camelCase** JSON.

## 2. Autentifikatsiya — `deviceId`

Token/parol yo'q. **Har bir so'rov tanasida `deviceId` yuboriladi** va u hisob hisoblanadi:

- Frontend bir marta barqaror `deviceId` (masalan UUID) yaratadi va lokalda saqlaydi (localStorage / SharedPreferences).
- Keyingi barcha so'rovlarda **aynan o'sha** `deviceId` ishlatiladi — backend shu orqali foydalanuvchini taniydi.

```js
let deviceId = localStorage.getItem("deviceId");
if (!deviceId) {
  deviceId = crypto.randomUUID();
  localStorage.setItem("deviceId", deviceId);
}
```

## 3. Ish oqimi

1. Ilova ochilganda **`POST /api/bootstrap/`** chaqiriladi → butun holat (`state`) qaytadi.
2. Boshqa amallar `{ success, message, state? }` qaytaradi. **Agar `state` kelsa — lokal holatni shu bilan to'liq almashtiring.**
3. Holat ichida: profil, oila a'zolari, bildirishnomalar, faollik tasmasi, SOS holati va h.k.

### Universal so'rov funksiyasi (fetch)

```js
const BASE = "https://gamxor-api.onrender.com/api/";

async function api(path, body = {}) {
  const res = await fetch(BASE + path, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ deviceId, ...body }),
  });
  return res.json();
}

// Misol:
const boot = await api("bootstrap/");
console.log(boot.state.members);          // oila a'zolari
const reg = await api("auth/request-code/", { phone: "+998901234567" });
```

## 4. Endpointlar

| Endpoint | Tana (device'dan tashqari) | Qaytaradi |
|---|---|---|
| `bootstrap/` | — | `{ state }` |
| `auth/request-code/` | `phone` | `{ success, message, state }` (demo kod `state.otpHint`da) |
| `auth/verify-code/` | `phone`, `code` | `{ success, message, state }` |
| `auth/register/` | `fullName`, `phone` | `{ success, message, state }` |
| `profile/save/` | `profile` (obyekt) | `{ success, message, state }` |
| `members/select/` | `memberId` | `{ success, message, state }` |
| `refresh/` | — | `{ success, message, state }` |
| `invitations/send/` | `name`, `relation`, `phone` | `{ success, message, state }` |
| `invitations/{id}/accept/` | — (id URL'da) | `{ success, message, state }` |
| `notifications/{id}/read/` | — | `{ success, message, state }` |
| `notifications/{id}/dismiss/` | — | `{ success, message, state }` |
| `notifications/read-all/` | — | `{ success, message, state }` |
| `sos/trigger/` | — | `{ success, message, state }` |
| `sos/clear/` | — | `{ success, message, state }` |
| `sign-out/` | — | `{ success, message, state }` |
| `uploads/assets/` | `multipart`: `deviceId`, `category`, `title`, `file` | `{ success, message, fileUrl }` |

### Autentifikatsiya oqimi (3 qadam)

```
auth/request-code/  → state.otpRequested=true, state.otpHint=<kod>
auth/verify-code/   → success bo'lsa raqam tasdiqlandi
auth/register/      → fullName bilan ro'yxat yakunlanadi → state.isLoggedIn=true
```
Demo rejimda SMS yuborilmaydi — kod `state.otpHint`da qaytadi (hozircha `2580`).

## 5. Enum qiymatlari (satr sifatida keladi)

| Maydon | Mumkin qiymatlar |
|---|---|
| `member.status` | `SAFE`, `MOVING`, `NEEDS_ATTENTION` |
| `activityFeed[].severity` | `POSITIVE`, `NEUTRAL`, `WARNING` |
| `invitation.status` | `PENDING_ACCEPTANCE`, `WAITING_INSTALL`, `ACCEPTED` |
| `notification.category` | `INVITE`, `SAFETY`, `CRIME`, `SYSTEM` |
| `sosState.contacts[].state` | `CALLING`, `NOTIFIED` |

## 6. Fayl (avatar) yuklash

```js
const fd = new FormData();
fd.append("deviceId", deviceId);
fd.append("category", "profile_avatar");
fd.append("title", "Profile avatar");
fd.append("file", fileInput.files[0]);

const res = await fetch(BASE + "uploads/assets/", { method: "POST", body: fd });
const { fileUrl } = await res.json();   // nisbiy yo'l, masalan /media/uploads/...
// To'liq URL: "https://gamxor-api.onrender.com" + fileUrl
```

## 7. Mavjud Flutter ilovasi

Flutter ilovasida API mijozi tayyor (`lib/data/api_service.dart`). Frontendchilar
faqat ilovadagi **server manzilini** quyidagiga o'zgartiradi:

```
https://gamxor-api.onrender.com/api/
```

Kod o'zgartirish shart emas.
