# 📮 MailDesk Backend — API Persuratan Digital

## 📌 Informasi Produk

| Aspek | Detail |
|-------|--------|
| **Framework** | ASP.NET Core 8.0 Web API |
| **Database** | PostgreSQL 16 |
| **Containerization** | Docker + Docker Compose |
| **API Documentation** | Swagger / OpenAPI 3.0 |
| **Port** | `5000` (Docker) · `5281` (lokal) |

---

## 🚀 Cara Menjalankan

### Dengan Docker (Recommended)

```bash
# Clone repository
git clone https://github.com/pens-pbl/maildesk-backend.git
cd maildesk-backend

# Jalankan
docker compose up --build

# Akses Swagger
http://localhost:5000/swagger
```

### Tanpa Docker (Development Lokal)

```bash
cd MailDesk.API
dotnet run

# Swagger: http://localhost:5281/swagger
```

---

## 📁 Struktur Folder

```
MailDesk/
├── MailDesk.API/
│   ├── Controllers/
│   │   └── SuratController.cs
│   ├── Services/
│   │   ├── Interfaces/
│   │   │   └── ISuratService.cs
│   │   └── SuratService.cs
│   ├── Entities/
│   │   ├── Surat.cs
│   │   ├── User.cs
│   │   ├── Role.cs
│   │   ├── Disposisi.cs
│   │   ├── DisposisiRelation.cs
│   │   ├── Inbox.cs
│   │   └── TemplateSurat.cs
│   ├── DTOs/
│   │   └── Surat/
│   │       ├── CreateSuratRequest.cs
│   │       ├── SuratResponse.cs
│   │       ├── SuratListResponse.cs
│   │       ├── SuratQueryParams.cs
│   │       ├── PaginatedResponse.cs
│   │       ├── UploadPdfResponse.cs
│   │       └── NomorAgendaPreviewResponse.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Helpers/
│   │   ├── NomorAgendaHelper.cs
│   │   └── SwaggerFileOperationFilter.cs
│   └── Program.cs
├── docker/
│   └── postgres/
│       ├── init.sql
│       └── seed.sql
├── docker-compose.yml
├── .env.example
├── .gitignore
└── README.md
```

---

## 🗄️ Database Schema

Tabel utama yang digunakan pada Sprint 1:

```
roles             → Data peran pengguna
users             → Data pengguna dengan email & password
surat             → Gabungan surat masuk dan surat keluar
disposisi         → Data disposisi surat
disposisi_relation → Relasi rantai disposisi (parent-child)
inbox             → Data penerimaan & forwarding surat
template_surat    → Template surat yang tersedia
```

### Tabel `surat` (Tabel Utama)

| Kolom | Tipe | Keterangan |
|-------|------|------------|
| `id` | SERIAL | Primary key |
| `no_surat` | VARCHAR(100) | Nomor surat resmi |
| `nomor_agenda` | VARCHAR(100) | Nomor agenda otomatis |
| `jenis_surat` | VARCHAR(20) | `Masuk` atau `Keluar` |
| `kategori_surat` | VARCHAR(50) | Undangan, Edaran, dll |
| `tanggal_surat` | DATE | Tanggal tertera di surat |
| `pengirim` | VARCHAR(150) | Nama instansi / pengirim |
| `penerima` | VARCHAR(150) | Nama penerima |
| `perihal` | TEXT | Perihal surat |
| `isi_teks_ocr` | TEXT | Teks hasil scan OCR |
| `nama_file` | VARCHAR(255) | Nama file PDF yang diupload |
| `status` | VARCHAR(20) | `Baru` / `Diproses` / `Selesai` |
| `is_archived` | BOOLEAN | Status arsip |
| `user_id` | INTEGER | FK → users.id (pencatat) |
| `created_at` | TIMESTAMP | Waktu pencatatan |

### Format Nomor Agenda

```
SM/YYYY/MM/XXX

Contoh: SM/2026/05/001
        SM/2026/05/002
```

---

## 📡 API Endpoints

### ── Task 1: Pencatatan Surat Masuk ──────────────────────────

#### `POST /api/surat`

Mencatat surat masuk baru beserta metadata ke database.

**Request Body** (`application/json`):

```json
{
  "noSurat": "421.3/B.1/DISDIK/2026",
  "tanggalSurat": "2026-05-01",
  "pengirim": "Dinas Pendidikan Kab. Bogor",
  "penerima": "Sekretaris",
  "perihal": "Undangan Workshop Digitalisasi",
  "kategoriSurat": "Undangan",
  "userId": 2,
  "nomorAgendaPreview": "SM/2026/05/001"
}
```

| Field | Tipe | Wajib | Keterangan |
|-------|------|-------|------------|
| `noSurat` | string | ✅ | Nomor surat resmi |
| `tanggalSurat` | date | ✅ | Format `YYYY-MM-DD` |
| `pengirim` | string | ✅ | Nama instansi pengirim |
| `penerima` | string | ✅ | Nama penerima |
| `perihal` | string | ✅ | Perihal / subject surat |
| `kategoriSurat` | string | ❌ | Undangan, Edaran, dll |
| `userId` | integer | ✅ | ID user yang mencatat |
| `nomorAgendaPreview` | string | ❌ | Nomor dari endpoint preview |

**Response `201 Created`**:

```json
{
  "success": true,
  "message": "Surat masuk berhasil dicatat.",
  "data": {
    "id": 1,
    "noSurat": "421.3/B.1/DISDIK/2026",
    "nomorAgenda": "SM/2026/05/001",
    "jenisSurat": "Masuk",
    "kategoriSurat": "Undangan",
    "tanggalSurat": "2026-05-01",
    "pengirim": "Dinas Pendidikan Kab. Bogor",
    "penerima": "Sekretaris",
    "perihal": "Undangan Workshop Digitalisasi",
    "status": "Baru",
    "hasLampiran": false,
    "pencatatNama": "Sekretaris",
    "createdAt": "2026-05-07T10:30:00Z"
  }
}
```

**Kemungkinan Error**:

| HTTP Code | Pesan | Penyebab |
|-----------|-------|----------|
| `400` | Field wajib tidak diisi | Validasi gagal |
| `400` | User tidak ditemukan | `userId` tidak valid |
| `500` | Terjadi kesalahan pada server | Internal error |

---

#### `GET /api/surat/nomor-agenda/preview`

Dipanggil saat halaman Input Surat Masuk **pertama kali dibuka**.  
Mengembalikan nomor agenda berikutnya sebagai preview sebelum disimpan.

> ⚠️ Nomor preview bersifat sementara. Jika ada user lain yang menyimpan surat lebih dulu, nomor final akan di-generate ulang secara otomatis saat `POST /api/surat` dipanggil.

**Response `200 OK`**:

```json
{
  "success": true,
  "data": {
    "nomorAgenda": "SM/2026/05/003",
    "keterangan": "Preview - nomor final dikonfirmasi saat simpan",
    "generatedAt": "2026-05-07T16:00:00Z"
  }
}
```

---

### ── Task 2: Upload File PDF ──────────────────────────────────

#### `POST /api/surat/{id}/upload-pdf`

Upload file PDF lampiran untuk surat yang sudah tercatat.

**Path Parameter**:

| Parameter | Tipe | Keterangan |
|-----------|------|------------|
| `id` | integer | ID surat yang sudah dicatat |

**Request** (`multipart/form-data`):

| Field | Tipe | Keterangan |
|-------|------|------------|
| `file` | file | File PDF yang akan diupload |

**Aturan Validasi File**:

```
✅ Format     : PDF only (.pdf)
✅ Max Size   : 10 MB
✅ MIME Type  : application/pdf
```

**Lokasi Penyimpanan File**:

```
Container : /app/wwwroot/uploads/surat/YYYY/MM/{guid}.pdf
Akses URL : http://localhost:5000/uploads/surat/2026/05/{guid}.pdf
```

**Response `200 OK`**:

```json
{
  "success": true,
  "message": "File PDF berhasil diupload.",
  "data": {
    "suratId": 1,
    "namaFile": "b3f4e1a2-9c8d-47b5-a1d0-2f3e4c5d6e7f.pdf",
    "fileSizeBytes": 524288,
    "fileSizeDisplay": "512 KB",
    "uploadedAt": "2026-05-07T10:35:00Z"
  }
}
```

**Kemungkinan Error**:

| HTTP Code | Pesan | Penyebab |
|-----------|-------|----------|
| `400` | Hanya file PDF yang diizinkan | Format bukan PDF |
| `400` | Ukuran file melebihi 10MB | File terlalu besar |
| `404` | Surat tidak ditemukan | ID tidak valid |
| `500` | Terjadi kesalahan pada server | Internal error |

---

### ── Task 3: Relasi Surat & Lampiran ─────────────────────────

#### `GET /api/surat/{id}`

Get detail surat beserta informasi lampiran PDF-nya.

**Path Parameter**:

| Parameter | Tipe | Keterangan |
|-----------|------|------------|
| `id` | integer | ID surat |

**Response `200 OK`**:

```json
{
  "success": true,
  "data": {
    "id": 1,
    "noSurat": "421.3/B.1/DISDIK/2026",
    "nomorAgenda": "SM/2026/05/001",
    "jenisSurat": "Masuk",
    "kategoriSurat": "Undangan",
    "tanggalSurat": "2026-05-01",
    "pengirim": "Dinas Pendidikan Kab. Bogor",
    "penerima": "Sekretaris",
    "perihal": "Undangan Workshop Digitalisasi",
    "status": "Baru",
    "isArchived": false,
    "pencatatNama": "Sekretaris",
    "namaFile": "b3f4e1a2-9c8d-47b5-a1d0-2f3e4c5d6e7f.pdf",
    "hasLampiran": true,
    "createdAt": "2026-05-07T10:30:00Z"
  }
}
```

**Kemungkinan Error**:

| HTTP Code | Pesan | Penyebab |
|-----------|-------|----------|
| `404` | Surat tidak ditemukan | ID tidak valid |
| `500` | Terjadi kesalahan pada server | Internal error |

---

### ── Dashboard: Inbox (Semua Surat) ─────────────────────────

#### `GET /api/surat`

Mengambil semua surat (masuk + keluar) untuk halaman **Dashboard / Inbox**.  
Mendukung pagination, sorting, filtering, dan pencarian.

**Query Parameters**:

| Parameter | Tipe | Default | Keterangan |
|-----------|------|---------|------------|
| `page` | integer | `1` | Nomor halaman |
| `limit` | integer | `10` | Jumlah item per halaman (max: 100) |
| `sortBy` | string | `tanggal` | `tanggal` · `nomor_agenda` · `pengirim` · `status` |
| `sortOrder` | string | `desc` | `asc` · `desc` |
| `search` | string | — | Cari di perihal / pengirim / no\_surat |
| `status` | string | — | `Baru` · `Diproses` · `Selesai` |
| `kategoriSurat` | string | — | Undangan, Edaran, dll |
| `tanggalDari` | date | — | Filter dari tanggal (`YYYY-MM-DD`) |
| `tanggalSampai` | date | — | Filter sampai tanggal (`YYYY-MM-DD`) |
| `includeArchived` | boolean | `false` | Tampilkan surat yang sudah diarsip |

**Contoh Request**:

```
GET /api/surat
GET /api/surat?page=1&limit=10
GET /api/surat?search=undangan
GET /api/surat?status=Baru&sortBy=tanggal&sortOrder=desc
GET /api/surat?tanggalDari=2026-05-01&tanggalSampai=2026-05-31
GET /api/surat?kategoriSurat=Undangan&page=2&limit=5
```

**Response `200 OK`**:

```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "noSurat": "421.3/B.1/DISDIK/2026",
      "nomorAgenda": "SM/2026/05/001",
      "jenisSurat": "Masuk",
      "kategoriSurat": "Undangan",
      "tanggalSurat": "2026-05-01",
      "pengirim": "Dinas Pendidikan Kab. Bogor",
      "penerima": "Sekretaris",
      "perihal": "Undangan Workshop Digitalisasi",
      "status": "Baru",
      "hasLampiran": false,
      "createdAt": "2026-05-07T10:30:00Z"
    },
    {
      "id": 2,
      "noSurat": "B-500/PUPR/2026",
      "nomorAgenda": "SM/2026/05/002",
      "jenisSurat": "Masuk",
      "kategoriSurat": null,
      "tanggalSurat": "2026-05-02",
      "pengirim": "Kementerian PUPR",
      "penerima": "Sekretaris",
      "perihal": "Rapat Koordinasi",
      "status": "Baru",
      "hasLampiran": true,
      "createdAt": "2026-05-07T11:00:00Z"
    }
  ],
  "meta": {
    "currentPage": 1,
    "totalPages": 3,
    "totalData": 25,
    "limit": 10,
    "hasNextPage": true,
    "hasPrevPage": false
  }
}
```

---

### ── Page Surat Masuk ─────────────────────────────────────────

#### `GET /api/surat/masuk`

Mengambil hanya surat dengan `jenis_surat = 'Masuk'` untuk halaman **Surat Masuk**.  
Mendukung query parameters yang sama dengan endpoint inbox.

**Contoh Request**:

```
GET /api/surat/masuk
GET /api/surat/masuk?page=1&limit=10
GET /api/surat/masuk?search=rapat
GET /api/surat/masuk?status=Baru
GET /api/surat/masuk?kategoriSurat=Undangan&sortOrder=asc
```

**Response `200 OK`**:

```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "noSurat": "421.3/B.1/DISDIK/2026",
      "nomorAgenda": "SM/2026/05/001",
      "jenisSurat": "Masuk",
      "kategoriSurat": "Undangan",
      "tanggalSurat": "2026-05-01",
      "pengirim": "Dinas Pendidikan Kab. Bogor",
      "penerima": "Sekretaris",
      "perihal": "Undangan Workshop Digitalisasi",
      "status": "Baru",
      "hasLampiran": false,
      "createdAt": "2026-05-07T10:30:00Z"
    }
  ],
  "meta": {
    "currentPage": 1,
    "totalPages": 2,
    "totalData": 15,
    "limit": 10,
    "hasNextPage": true,
    "hasPrevPage": false
  }
}
```

---

### ── Page Surat Keluar ────────────────────────────────────────

#### `GET /api/surat/keluar`

Mengambil hanya surat dengan `jenis_surat = 'Keluar'` untuk halaman **Surat Keluar**.  
Mendukung query parameters yang sama dengan endpoint inbox.

**Contoh Request**:

```
GET /api/surat/keluar
GET /api/surat/keluar?page=1&limit=10
GET /api/surat/keluar?status=Selesai
GET /api/surat/keluar?sortBy=pengirim&sortOrder=asc
```

**Response `200 OK`**:

```json
{
  "success": true,
  "data": [
    {
      "id": 10,
      "noSurat": "001/TU/MAILDESK/V/2026",
      "nomorAgenda": "SK/2026/05/001",
      "jenisSurat": "Keluar",
      "kategoriSurat": "Edaran",
      "tanggalSurat": "2026-05-05",
      "pengirim": "Sekretaris",
      "penerima": "Seluruh Divisi",
      "perihal": "Edaran Libur Nasional",
      "status": "Selesai",
      "hasLampiran": false,
      "createdAt": "2026-05-05T09:00:00Z"
    }
  ],
  "meta": {
    "currentPage": 1,
    "totalPages": 1,
    "totalData": 8,
    "limit": 10,
    "hasNextPage": false,
    "hasPrevPage": false
  }
}
```

---

## 📊 Ringkasan Endpoint

| Method | Endpoint | Kegunaan | Task |
|--------|----------|----------|------|
| `GET` | `/api/surat/nomor-agenda/preview` | Generate preview nomor agenda | Task 1 |
| `POST` | `/api/surat` | Catat surat masuk baru | Task 1 |
| `POST` | `/api/surat/{id}/upload-pdf` | Upload lampiran PDF | Task 2 |
| `GET` | `/api/surat/{id}` | Detail surat + lampiran | Task 3 |
| `GET` | `/api/surat` | Semua surat (Dashboard / Inbox) | Task 3 |
| `GET` | `/api/surat/masuk` | Daftar surat masuk | Task 3 |
| `GET` | `/api/surat/keluar` | Daftar surat keluar | Task 3 |

---

## 🔗 Akses

| Layanan | URL |
|---------|-----|
| **Swagger UI** | http://localhost:5000/swagger |
| **API Base URL** | http://localhost:5000/api |
| **PostgreSQL** | localhost:5432 |
| **pgAdmin / DBeaver** | Host: `localhost` · Port: `5432` · DB: `maildesk_db` |

**Database Credentials**:

```
Database : maildesknew_db
Username : postgres
Password : (password)
```

---

## 🐳 Docker Commands

```bash
# Jalankan pertama kali / setelah ada perubahan kode
docker compose up --build

# Jalankan di background
docker compose up -d

# Stop container
docker compose down

# Reset total (hapus database & volume)
docker compose down -v
docker compose up --build

# Lihat log API
docker logs maildesk_api

# Masuk ke database PostgreSQL
docker exec -it maildesk_postgres psql -U maildesk_user -d maildesk_db
```

---

## 🧪 Contoh Testing via cURL

```bash
# GET nomor agenda preview
curl http://localhost:5000/api/surat/nomor-agenda/preview

# POST surat masuk
curl -X POST http://localhost:5000/api/surat \
  -H "Content-Type: application/json" \
  -d '{
    "noSurat": "421.3/B.1/DISDIK/2026",
    "tanggalSurat": "2026-05-07",
    "pengirim": "Dinas Pendidikan",
    "penerima": "Sekretaris",
    "perihal": "Undangan Rapat",
    "userId": 2
  }'

# GET semua surat (inbox / dashboard)
curl "http://localhost:5000/api/surat?page=1&limit=10"

# GET surat masuk saja
curl "http://localhost:5000/api/surat/masuk?page=1&limit=10"

# GET surat keluar saja
curl "http://localhost:5000/api/surat/keluar?page=1&limit=10"

# GET detail surat
curl http://localhost:5000/api/surat/1

# Upload PDF
curl -X POST http://localhost:5000/api/surat/1/upload-pdf \
  -F "file=@/path/to/file.pdf"
```
---

**Last Updated**: 7 Mei 2026 · **Sprint**: 1 · **Version**: 1.0.0-sprint1