# MailDesk — Sistem Persuratan Digital

## Struktur Project
- Backend: ASP.NET Core 8.0
- Database: PostgreSQL
- Containerization: Docker & Docker Compose

## Quick Start

### Prerequisites
- Docker & Docker Compose installed
- Git

### Setup (Local Development)

1. **Clone repository**
```bash
   git clone https://github.com/username/MailDesk.git
   cd MailDesk
```

2. **Copy environment file**
```bash
   cp .env.example .env
```

3. **Edit `.env` sesuai preferensi lokal**
```bash
   nano .env
```

4. **Jalankan Docker Compose**
```bash
   docker compose up --build
```

5. **API Swagger**
   - http://localhost:5000/swagger

6. **Database Connection**
   - Host: localhost:5432
   - Username: maildesk_user
   - Database: maildesk_db
   - Password: (lihat di .env)

## API Endpoints (Sprint 1)

### Surat Masuk
- **POST** `/api/suratmasuk` — Catat surat masuk baru
- **GET** `/api/suratmasuk/{id}` — Get detail surat

## Development Team

| Nama | Role | Sprint 1 Tasks |
|------|------|---|
| Efra | Backend | Surat Masuk API + Tracking |
| Akmal | Backend | GET API + Filter |
| Lizzy | Backend | Approval API |

## Troubleshooting

### Database connection refused
```bash
# Pastikan postgres service sudah ready
docker compose logs postgres

# Reset database
docker compose down -v
docker compose up
```

### Port 5000 sudah terpakai
Edit `.env`:
```env
API_PORT=5001
```

---

Untuk informasi lebih, lihat wiki atau hubungi tim backend.