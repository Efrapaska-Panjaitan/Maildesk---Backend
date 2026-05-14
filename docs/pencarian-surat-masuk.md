# Dokumentasi Fitur Pencarian Surat Masuk

Fitur pencarian surat masuk digunakan untuk mengambil daftar data surat masuk dari database melalui endpoint `GET /api/surat-masuk`.

Fitur ini mendukung:

- pencarian data surat masuk
- filter berdasarkan beberapa parameter
- sorting data
- pagination
- validasi request
- optimasi query menggunakan indexing database

---

## Endpoint

```http
GET /api/surat-masuk

Endpoint ini digunakan untuk mengambil daftar surat masuk berdasarkan parameter pencarian tertentu.

Query Parameter
Parameter	Keterangan
nomorAgenda	Mencari surat berdasarkan nomor agenda
nomorSurat	Mencari surat berdasarkan nomor surat
namaPengirim	Mencari surat berdasarkan nama pengirim
perihal	Mencari surat berdasarkan perihal surat
status	Mencari surat berdasarkan status surat
jenisSumber	Mencari surat berdasarkan jenis sumber surat
tanggalDiterimaDari	Filter tanggal diterima dari tanggal tertentu
tanggalDiterimaSampai	Filter tanggal diterima sampai tanggal tertentu
sortBy	Field yang digunakan untuk sorting
sortDirection	Arah sorting, yaitu asc atau desc
page	Nomor halaman
pageSize	Jumlah data per halaman
Sorting

Field yang dapat digunakan untuk sortBy:

Nilai sortBy	Keterangan
tanggal_diterima	Sorting berdasarkan tanggal diterima
nomor_agenda	Sorting berdasarkan nomor agenda
nomor_surat	Sorting berdasarkan nomor surat
tanggal_surat	Sorting berdasarkan tanggal surat
nama_pengirim	Sorting berdasarkan nama pengirim
status	Sorting berdasarkan status surat
jenis_sumber	Sorting berdasarkan jenis sumber surat

Nilai yang dapat digunakan untuk sortDirection:

asc
desc
Contoh Request
Mengambil data dengan pagination
GET /api/surat-masuk?page=1&pageSize=10
Sorting berdasarkan tanggal diterima
GET /api/surat-masuk?sortBy=tanggal_diterima&sortDirection=desc&page=1&pageSize=10
Sorting berdasarkan nomor agenda
GET /api/surat-masuk?sortBy=nomor_agenda&sortDirection=asc&page=1&pageSize=10
Sorting berdasarkan jenis sumber
GET /api/surat-masuk?sortBy=jenis_sumber&sortDirection=asc&page=1&pageSize=10
Filter berdasarkan status
GET /api/surat-masuk?status=diterima&page=1&pageSize=10
Filter berdasarkan nomor agenda
GET /api/surat-masuk?nomorAgenda=AGD&page=1&pageSize=10
Filter berdasarkan rentang tanggal diterima
GET /api/surat-masuk?tanggalDiterimaDari=2026-04-01&tanggalDiterimaSampai=2026-04-30&page=1&pageSize=10
Format Response

Response menggunakan format pagination dengan properti data dan meta.

Contoh response:

{
  "data": [
    {
      "id": 1,
      "nomorAgenda": "AGD-001",
      "nomorSurat": "001/DISDIK/IV/2026",
      "tanggalSurat": "2026-04-20",
      "tanggalDiterima": "2026-04-28",
      "namaPengirim": "Dinas Pendidikan",
      "instansiPengirim": "Dinas Pendidikan Kota",
      "perihal": "Undangan Rapat Koordinasi",
      "jenisSumber": "eksternal",
      "status": "diterima"
    }
  ],
  "meta": {
    "page": 1,
    "pageSize": 10,
    "totalData": 1,
    "totalPage": 1,
    "sortBy": "tanggal_diterima",
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
tanggalDiterimaDari tidak boleh lebih besar dari tanggalDiterimaSampai.
page dan pageSize digunakan untuk membatasi jumlah data yang dikembalikan.

Contoh request tidak valid:

GET /api/surat-masuk?sortBy=random_field

Contoh response:

{
  "message": "sortBy tidak valid."
}

Contoh request tanggal tidak valid:

GET /api/surat-masuk?tanggalDiterimaDari=2026-04-30&tanggalDiterimaSampai=2026-04-01

Contoh response:

{
  "message": "TanggalDiterimaDari tidak boleh lebih besar dari TanggalDiterimaSampai."
}
Optimasi Database

Untuk meningkatkan performa pencarian dan pengurutan data, tabel surat_masuk sudah ditambahkan index pada beberapa kolom yang sering digunakan.

Index yang ditambahkan:

Kolom	Kegunaan
nomor_agenda	Mempercepat pencarian berdasarkan nomor agenda
tanggal_diterima	Mempercepat filter dan sorting berdasarkan tanggal diterima
tanggal_diterima, nomor_agenda	Mengoptimalkan pencarian kombinasi tanggal diterima dan nomor agenda
status	Mempercepat pencarian berdasarkan status surat
jenis_sumber	Mempercepat pencarian dan sorting berdasarkan jenis sumber

Konfigurasi index pada MaildeskDbcontext.cs:

entity.HasIndex(x => x.NomorAgenda);
entity.HasIndex(x => x.TanggalDiterima);
entity.HasIndex(x => new { x.TanggalDiterima, x.NomorAgenda });
entity.HasIndex(x => x.Status);
entity.HasIndex(x => x.JenisSumber);

Migration yang digunakan:

AddIndexSuratMasukSearch

Command migration:

dotnet ef migrations add AddIndexSuratMasukSearch --project Maildesk.Api --startup-project Maildesk.Api
dotnet ef database update --project Maildesk.Api --startup-project Maildesk.Api

Catatan:

Database sebelumnya sudah memiliki tabel surat_masuk, sehingga migration disesuaikan agar hanya menambahkan index dan tidak membuat ulang tabel.

File yang Berhubungan

File utama yang berhubungan dengan fitur pencarian surat masuk:

Maildesk.Api/Data/MaildeskDbcontext.cs
Maildesk.Api/Services/SuratMasukService.cs
Maildesk.Api/Dtos/SuratMasukQueryDto.cs
Maildesk.Api/Dtos/PagedResponseDto.cs
Maildesk.Api/Migrations/...
Testing Menggunakan Swagger

Jalankan project:

dotnet run --project Maildesk.Api

Buka Swagger:

http://localhost:5175/swagger/index.html

Endpoint yang diuji:

GET /api/surat-masuk

Contoh pengujian:

GET /api/surat-masuk?page=1&pageSize=10&sortBy=tanggal_diterima&sortDirection=desc

Expected result:

200 OK

Validasi sorting tidak valid:

GET /api/surat-masuk?sortBy=random_field

Expected result:

400 Bad Request
Pengembangan Berikutnya

Pengembangan berikutnya adalah menambahkan fitur input surat masuk melalui endpoint:

POST /api/surat-masuk

Fitur yang akan ditambahkan:

DTO untuk input surat masuk
method create pada service
endpoint POST pada controller
validasi field wajib
testing input data melalui Swagger
penyimpanan data ke database PostgreSQL

File yang akan ditambahkan:

Maildesk.Api/Dtos/CreateSuratMasukDto.cs

File yang akan diperbarui:

Maildesk.Api/Services/SuratMasukService.cs
Maildesk.Api/Controllers/SuratMasukController.cs