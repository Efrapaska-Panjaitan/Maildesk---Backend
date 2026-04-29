# Maildesk API

Maildesk API adalah backend untuk sistem pengelolaan surat yang dibuat menggunakan ASP.NET Core Web API, Entity Framework Core, dan PostgreSQL. Project ini digunakan untuk mengelola data surat masuk dan surat keluar, termasuk fitur pencarian, filter, sorting, pagination, validasi request, serta optimasi query database menggunakan indexing.

## Teknologi

- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Swagger UI

## Menjalankan Project

Jalankan project dari folder `Maildesk.Api`:

```bash
dotnet run

Swagger dapat diakses melalui:

http://localhost:5175/swagger/index.html
Fitur yang Sudah Dibuat

Fitur yang sudah dibuat saat ini adalah pencarian data surat masuk dan surat keluar melalui endpoint GET. Kedua fitur ini memiliki pola yang sama, yaitu mendukung filter data, sorting, pagination, response metadata, validasi request, dan indexing database untuk optimasi pencarian.

Pencarian Surat Masuk

Fitur pencarian surat masuk digunakan untuk mengambil daftar data surat masuk dari database melalui endpoint:

GET /api/surat-masuk

Endpoint ini mendukung pencarian berdasarkan:

nomor agenda
nomor surat
nama pengirim
perihal
status
jenis sumber
rentang tanggal diterima
Query Parameter Surat Masuk
Parameter	Keterangan
nomorAgenda	Mencari surat masuk berdasarkan nomor agenda
nomorSurat	Mencari surat masuk berdasarkan nomor surat
namaPengirim	Mencari surat masuk berdasarkan nama pengirim
perihal	Mencari surat masuk berdasarkan perihal
status	Mencari surat masuk berdasarkan status
jenisSumber	Mencari surat masuk berdasarkan jenis sumber
tanggalDiterimaDari	Filter tanggal diterima dari tanggal tertentu
tanggalDiterimaSampai	Filter tanggal diterima sampai tanggal tertentu
sortBy	Field yang digunakan untuk sorting
sortDirection	Arah sorting, yaitu asc atau desc
page	Nomor halaman
pageSize	Jumlah data per halaman
Sorting Surat Masuk

Field sortBy yang tersedia:

Nilai	Keterangan
tanggal_diterima	Sorting berdasarkan tanggal diterima
nomor_agenda	Sorting berdasarkan nomor agenda
nomor_surat	Sorting berdasarkan nomor surat
tanggal_surat	Sorting berdasarkan tanggal surat
nama_pengirim	Sorting berdasarkan nama pengirim
status	Sorting berdasarkan status
jenis_sumber	Sorting berdasarkan jenis sumber

Contoh request:

GET /api/surat-masuk?page=1&pageSize=10&sortBy=tanggal_diterima&sortDirection=desc
GET /api/surat-masuk?nomorSurat=001&page=1&pageSize=10
GET /api/surat-masuk?tanggalDiterimaDari=2026-04-01&tanggalDiterimaSampai=2026-04-30&page=1&pageSize=10
Optimasi Surat Masuk

Untuk optimasi pencarian, tabel surat_masuk ditambahkan index pada:

nomor_agenda
tanggal_diterima
kombinasi tanggal_diterima dan nomor_agenda
status
jenis_sumber
Pencarian Surat Keluar

Fitur pencarian surat keluar digunakan untuk mengambil daftar data surat keluar dari database melalui endpoint:

GET /api/surat-keluar

Endpoint ini dibuat dengan pola yang sama seperti surat masuk, tetapi menyesuaikan struktur asli tabel surat_keluar pada database PostgreSQL.

Endpoint ini mendukung pencarian berdasarkan:

nomor agenda
nomor surat
tujuan surat
instansi tujuan
perihal
status
rentang tanggal surat
Query Parameter Surat Keluar
Parameter	Keterangan
nomorAgenda	Mencari surat keluar berdasarkan nomor agenda
nomorSurat	Mencari surat keluar berdasarkan nomor surat
tujuanSurat	Mencari surat keluar berdasarkan tujuan surat
instansiTujuan	Mencari surat keluar berdasarkan instansi tujuan
perihal	Mencari surat keluar berdasarkan perihal
status	Mencari surat keluar berdasarkan status
tanggalSuratDari	Filter tanggal surat dari tanggal tertentu
tanggalSuratSampai	Filter tanggal surat sampai tanggal tertentu
sortBy	Field yang digunakan untuk sorting
sortDirection	Arah sorting, yaitu asc atau desc
page	Nomor halaman
pageSize	Jumlah data per halaman
Sorting Surat Keluar

Field sortBy yang tersedia:

Nilai	Keterangan
tanggal_surat	Sorting berdasarkan tanggal surat
nomor_agenda	Sorting berdasarkan nomor agenda
nomor_surat	Sorting berdasarkan nomor surat
tujuan_surat	Sorting berdasarkan tujuan surat
instansi_tujuan	Sorting berdasarkan instansi tujuan
perihal	Sorting berdasarkan perihal
status	Sorting berdasarkan status surat

Contoh request:

GET /api/surat-keluar?page=1&pageSize=10&sortBy=tanggal_surat&sortDirection=desc
GET /api/surat-keluar?nomorAgenda=AGD&page=1&pageSize=10
GET /api/surat-keluar?tujuanSurat=Akmal&page=1&pageSize=10
GET /api/surat-keluar?tanggalSuratDari=2026-04-01&tanggalSuratSampai=2026-04-30&page=1&pageSize=10
Struktur Tabel Surat Keluar

Struktur tabel surat_keluar yang digunakan:

id
nomor_agenda
nomor_surat
tanggal_surat
tujuan_surat
instansi_tujuan
perihal
isi_ringkas
kode_klasifikasi
tingkat_prioritas
status
dibuat_oleh
diperbarui_oleh
dibuat_pada
diperbarui_pada

Kolom nomor_agenda ditambahkan melalui migration agar fitur pencarian surat keluar dapat disamakan dengan pola pencarian surat masuk.

Optimasi Surat Keluar

Untuk optimasi pencarian, tabel surat_keluar ditambahkan index pada:

nomor_agenda
nomor_surat
tanggal_surat
kombinasi tanggal_surat dan nomor_agenda
status
tujuan_surat
Format Response

Endpoint pencarian surat masuk dan surat keluar menggunakan format response pagination dengan properti data dan meta.

Contoh response:

{
  "data": [
    {
      "id": 1,
      "nomorAgenda": "AGD-001",
      "nomorSurat": "001/MAIL/IV/2026",
      "tanggalSurat": "2026-04-28",
      "perihal": "Contoh Surat",
      "status": "draft"
    }
  ],
  "meta": {
    "page": 1,
    "pageSize": 10,
    "totalData": 1,
    "totalPage": 1,
    "sortBy": "tanggal_surat",
    "sortDirection": "desc"
  }
}
Metadata Response
Field	Keterangan
page	Halaman yang sedang diakses
pageSize	Jumlah data per halaman
totalData	Total data yang sesuai filter
totalPage	Total halaman berdasarkan jumlah data
sortBy	Field sorting yang digunakan
sortDirection	Arah sorting yang digunakan
Validasi Request

Validasi request diterapkan agar parameter pencarian tetap sesuai format dan tidak menyebabkan error pada sistem.

Validasi yang diterapkan:

sortBy hanya boleh menggunakan field yang sudah didukung.
sortDirection hanya boleh bernilai asc atau desc.
Rentang tanggal tidak boleh terbalik.
page dan pageSize digunakan untuk membatasi jumlah data yang dikembalikan.

Contoh request tidak valid:

GET /api/surat-keluar?sortBy=random_field

Contoh response:

{
  "message": "sortBy tidak valid."
}
File yang Berhubungan

File utama yang berhubungan dengan fitur pencarian surat masuk dan surat keluar:

Controllers/SuratMasukController.cs
Controllers/SuratKeluarController.cs
Data/MaildeskDbcontext.cs
Dtos/SuratMasukQueryDto.cs
Dtos/SuratMasukItemDto.cs
Dtos/SuratKeluarQueryDto.cs
Dtos/SuratKeluarItemDto.cs
Dtos/PagedResponseDto.cs
Entities/SuratMasuk.cs
Entities/SuratKeluar.cs
Services/SuratMasukService.cs
Services/SuratKeluarService.cs
Migrations/...
docs/pencarian-surat-masuk.md
docs/pencarian-surat-keluar.md
Dokumentasi Detail

Dokumentasi detail fitur tersedia di folder docs:

docs/pencarian-surat-masuk.md
docs/pencarian-surat-keluar.md
Pengembangan Berikutnya

Pengembangan berikutnya adalah menambahkan fitur input data melalui endpoint:

POST /api/surat-masuk
POST /api/surat-keluar

Fitur yang akan ditambahkan:

DTO untuk input surat
method create pada service
endpoint POST pada controller
validasi field wajib
testing input data melalui Swagger
penyimpanan data ke database PostgreSQL

Setelah paste, klik **Commit changes...** di GitHub. Pesan commit yang cocok:

```text
docs: add main project readme
