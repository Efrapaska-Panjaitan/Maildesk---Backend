# Dokumentasi Fitur Pencarian Surat Keluar

Fitur pencarian surat keluar digunakan untuk mengambil daftar data surat keluar dari database melalui endpoint `GET /api/surat-keluar`. Fitur ini dibuat mengikuti pola pencarian surat masuk, yaitu mendukung filter, sorting, pagination, validasi request, metadata response, serta optimasi query menggunakan indexing database.

## Endpoint

```http
GET /api/surat-keluar

Endpoint ini digunakan untuk mengambil daftar surat keluar berdasarkan parameter pencarian tertentu.

Query Parameter
Parameter	Keterangan
nomorAgenda	Mencari surat keluar berdasarkan nomor agenda
nomorSurat	Mencari surat keluar berdasarkan nomor surat
tujuanSurat	Mencari surat keluar berdasarkan tujuan surat
instansiTujuan	Mencari surat keluar berdasarkan instansi tujuan
perihal	Mencari surat keluar berdasarkan perihal
status	Mencari surat keluar berdasarkan status surat
tanggalSuratDari	Filter tanggal surat dari tanggal tertentu
tanggalSuratSampai	Filter tanggal surat sampai tanggal tertentu
sortBy	Field yang digunakan untuk sorting
sortDirection	Arah sorting, yaitu asc atau desc
page	Nomor halaman
pageSize	Jumlah data per halaman
Sorting

Field yang dapat digunakan untuk sortBy:

Nilai sortBy	Keterangan
tanggal_surat	Sorting berdasarkan tanggal surat
nomor_agenda	Sorting berdasarkan nomor agenda
nomor_surat	Sorting berdasarkan nomor surat
tujuan_surat	Sorting berdasarkan tujuan surat
instansi_tujuan	Sorting berdasarkan instansi tujuan
perihal	Sorting berdasarkan perihal
status	Sorting berdasarkan status surat

Nilai yang dapat digunakan untuk sortDirection:

asc
desc
Contoh Request

Mengambil data surat keluar dengan pagination:

GET /api/surat-keluar?page=1&pageSize=10

Sorting berdasarkan tanggal surat:

GET /api/surat-keluar?sortBy=tanggal_surat&sortDirection=desc&page=1&pageSize=10

Sorting berdasarkan nomor agenda:

GET /api/surat-keluar?sortBy=nomor_agenda&sortDirection=asc&page=1&pageSize=10

Filter berdasarkan nomor agenda:

GET /api/surat-keluar?nomorAgenda=AGD&page=1&pageSize=10

Filter berdasarkan nomor surat:

GET /api/surat-keluar?nomorSurat=001&page=1&pageSize=10

Filter berdasarkan tujuan surat:

GET /api/surat-keluar?tujuanSurat=Akmal&page=1&pageSize=10

Filter berdasarkan instansi tujuan:

GET /api/surat-keluar?instansiTujuan=Dinas&page=1&pageSize=10

Filter berdasarkan status:

GET /api/surat-keluar?status=draft&page=1&pageSize=10

Filter berdasarkan rentang tanggal surat:

GET /api/surat-keluar?tanggalSuratDari=2026-04-01&tanggalSuratSampai=2026-04-30&page=1&pageSize=10
Format Response

Response menggunakan format pagination dengan properti data dan meta.

Contoh response:

{
  "data": [
    {
      "id": 1,
      "nomorAgenda": "AGD-001",
      "nomorSurat": "001/MAIL/IV/2026",
      "tanggalSurat": "2026-04-28",
      "tujuanSurat": "Akmal Fari",
      "instansiTujuan": "Yumilek",
      "perihal": "Surat Keluar",
      "isiRingkas": "Ringkasan isi surat keluar",
      "kodeKlasifikasi": "SK",
      "tingkatPrioritas": "normal",
      "status": "draft",
      "dibuatPada": "2026-04-28T10:00:00",
      "diperbaruiPada": "2026-04-28T10:00:00"
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

Validasi request digunakan agar parameter pencarian tetap sesuai format dan tidak menyebabkan error pada sistem.

Validasi yang diterapkan:

sortBy hanya boleh menggunakan field yang sudah didukung.
sortDirection hanya boleh bernilai asc atau desc.
tanggalSuratDari tidak boleh lebih besar dari tanggalSuratSampai.
page dan pageSize digunakan untuk membatasi jumlah data yang dikembalikan.

Contoh request tidak valid:

GET /api/surat-keluar?sortBy=random_field

Contoh response:

{
  "message": "sortBy tidak valid."
}
Struktur Database Surat Keluar

Tabel surat_keluar menggunakan kolom utama berikut:

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

Kolom nomor_agenda ditambahkan melalui migration agar pola pencarian surat keluar dapat disamakan dengan pencarian surat masuk.

Optimasi Database

Untuk meningkatkan performa pencarian dan pengurutan data, tabel surat_keluar ditambahkan index pada beberapa kolom yang sering digunakan.

Index yang ditambahkan:

Kolom	Kegunaan
nomor_agenda	Mempercepat pencarian berdasarkan nomor agenda
nomor_surat	Mempercepat pencarian berdasarkan nomor surat
tanggal_surat	Mempercepat filter dan sorting berdasarkan tanggal surat
tanggal_surat, nomor_agenda	Mengoptimalkan pencarian kombinasi tanggal surat dan nomor agenda
status	Mempercepat pencarian berdasarkan status surat
tujuan_surat	Mempercepat pencarian berdasarkan tujuan surat

Konfigurasi index pada MaildeskDbcontext.cs:

entity.HasIndex(x => x.NomorAgenda);
entity.HasIndex(x => x.NomorSurat);
entity.HasIndex(x => x.TanggalSurat);
entity.HasIndex(x => new { x.TanggalSurat, x.NomorAgenda });
entity.HasIndex(x => x.Status);
entity.HasIndex(x => x.TujuanSurat);

Migration yang digunakan:

FixSuratKeluarSearchColumns

Command migration:

dotnet ef migrations add FixSuratKeluarSearchColumns --project Maildesk.Api --startup-project Maildesk.Api
dotnet ef database update --project Maildesk.Api --startup-project Maildesk.Api

Catatan: tabel surat_keluar sebelumnya sudah ada di database PostgreSQL, sehingga migration disesuaikan agar tidak membuat ulang tabel dan hanya menambahkan kolom nomor_agenda serta index pencarian.

File yang Berhubungan

File utama yang berhubungan dengan fitur pencarian surat keluar:

Entities/SuratKeluar.cs
Data/MaildeskDbcontext.cs
Dtos/SuratKeluarQueryDto.cs
Dtos/SuratKeluarItemDto.cs
Services/SuratKeluarService.cs
Controllers/SuratKeluarController.cs
Migrations/...
Testing Menggunakan Swagger

Jalankan project:

dotnet run

Buka Swagger:

http://localhost:5175/swagger/index.html

Endpoint yang diuji:

GET /api/surat-keluar

Contoh pengujian:

GET /api/surat-keluar?page=1&pageSize=10&sortBy=tanggal_surat&sortDirection=desc

Expected result:

200 OK

Validasi sorting tidak valid:

GET /api/surat-keluar?sortBy=random_field

Expected result:

400 Bad Request
Pengembangan Berikutnya

Pengembangan berikutnya adalah menambahkan fitur input surat keluar melalui endpoint:

POST /api/surat-keluar

Fitur yang akan ditambahkan:

DTO untuk input surat keluar
method create pada service
endpoint POST pada controller
validasi field wajib
testing input data melalui Swagger
penyimpanan data ke database PostgreSQL

---

## 3. Commit dokumentasi

Karena repo kamu ada di `Maildesk.Api`, jalankan dari sini:

```bash
cd C:\Maildesk-app\Maildesk.Api
git status
git add .
git commit -m "docs: add surat keluar search documentation"