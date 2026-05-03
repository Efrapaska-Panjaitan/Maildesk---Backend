# MailDesk — Sistem Persuratan Digital

Backend untuk platform registrasi surat masuk/keluar, disposisi berjenjang, nomor surat otomatis, arsip digital, dan tracking disposisi.

## 📋 Project Info

- **Framework**: ASP.NET Core 8.0 Web API
- **Database**: PostgreSQL 16
- **Containerization**: Docker & Docker Compose
- **Architecture**: Clean Architecture + Service Layer Pattern
- **Sprint**: Sprint 1 (MVP)

## 👥 Team

| Nama | Role | Tasks |
|------|------|-------|
| Efra | Backend Developer | Task 1,2,3,4,5,6 - API Surat Masuk, Upload PDF, Tracking |
| Akmal | Backend Developer | Task 7,8,9 - GET API, Filter, Search, Authorization |
| Lizzy | Backend Developer | Task 10,11 - Approval, Status Change |

## 📋 Sprint 1 Deliverables

### ✅ Completed (Efra)
- [x] **Task 1**: API Pencatatan Surat Masuk dengan metadata
- [x] **Task 2**: Upload File PDF dengan validasi
- [x] **Task 3**: Relasi Database untuk File Lampiran

### 🔄 In Progress
- [ ] **Task 4**: API Tracking Disposisi
- [ ] **Task 5**: Log History Disposisi
- [ ] **Task 6**: Endpoint Detail Tracking

## 🚀 Quick Start

### Prerequisites
- Docker & Docker Compose
- Git

### Setup & Run

1. **Clone Repository**
```bash
   git clone https://github.com/Efrapaska-Panjaitan/Maildesk---Backend.git
   cd Maildesk---Backend
```

2. **Setup Environment**
```bash
   cp .env.example .env
   nano .env  # Edit password sesuai kebutuhan
```

3. **Run Docker**
```bash
   docker compose up --build
```

4. **Access API**
   - Swagger UI: http://localhost:5000/swagger
   - API Base: http://localhost:5000/api

5. **Access Database**
```bash
   docker exec -it maildesk_postgres psql -U maildesk_user -d maildesk_db
```

## 📚 API Documentation

### Surat Masuk Endpoints

**POST** `/api/suratmasuk` — Catat surat masuk baru
```json
{
  "noSurat": "421.3/B.1/DISDIK/2026",
  "tanggalSurat": "2026-05-02",
  "asalPengirim": "Dinas Pendidikan Kab. Bogor",
  "perihal": "Undangan Workshop",
  "userId": 2
}
```

Response:
```json
{
  "success": true,
  "message": "Surat masuk berhasil dicatat.",
  "data": {
    "id": 1,
    "noSurat": "421.3/B.1/DISDIK/2026",
    "nomorAgenda": "SM/2026/05/001",
    "tanggalSurat": "2026-05-02",
    "asalPengirim": "Dinas Pendidikan Kab. Bogor",
    "perihal": "Undangan Workshop",
    "nomorAgendaAsli": "-",
    "isArchived": false,
    "pencatatNama": "Budi Santoso",
    "namaFile": null,
    "filePath": null,
    "createdAt": "2026-05-02T10:30:00Z"
  }
}
```

**POST** `/api/suratmasuk/{id}/upload-pdf` — Upload lampiran PDF
- **Content-Type**: `multipart/form-data`
- **Parameter**: `id` (path), `file` (form)
- **File Size**: Max 10MB
- **File Type**: PDF only

## 🔒 Security Notes

- **`.env`**: JANGAN commit ke Git (sudah di `.gitignore`)
- **Credentials**: Gunakan `.env.example` sebagai template
- **API Keys**: Akan ditambahkan di sprint berikutnya

## 🧪 Testing

### Via Swagger
http://localhost:5000/swagger

### Via cURL
```bash
curl -X POST http://localhost:5000/api/suratmasuk \
  -H "Content-Type: application/json" \
  -d '{
    "noSurat": "TEST/2026/001",
    "tanggalSurat": "2026-05-02",
    "asalPengirim": "Test Department",
    "perihal": "Test Letter",
    "userId": 2
  }'
```

## 📦 Docker Hub

Prebuilt images tersedia di Docker Hub:
```bash
docker pull efvinn/maildesk-api:sprint1
```

## 🐛 Troubleshooting

### Port 5000 sudah dipakai
Edit `.env`:
```env
API_PORT=5001
```

### Database connection refused
```bash
docker compose down -v
docker compose up
```

### File upload error
Pastikan folder `wwwroot/uploads/` ada:
```bash
mkdir -p MailDesk.API/wwwroot/uploads/surat-masuk
```

## 📝 Git Workflow

```bash
# Create branch untuk feature baru
git checkout -b feature/task-4-tracking

# Commit dengan message yang descriptive
git commit -m "feat: implement tracking disposisi API"

# Push ke GitHub
git push origin feature/task-4-tracking

# Buat Pull Request di GitHub
```


**Last Updated**: May 3, 2026
**Sprint**: 1 (Active Development)