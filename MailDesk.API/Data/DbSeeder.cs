using Maildesk.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maildesk.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(MaildeskDbContext db)
    {
        await db.Database.MigrateAsync();

        if (await db.SuratMasuk.AnyAsync()) return;

        var pengirimList = new[]
        {
            "Dinas Kesehatan", "Kementerian Keuangan", "Bappeda Kota",
            "Dinas Pendidikan", "Sekretariat Daerah", "Pemerintah Kota",
            "BPJS Kesehatan", "Kantor Wilayah Kemenkumham"
        };

        var perihalList = new[]
        {
            "Undangan rapat pembangunan", "Permohonan data pegawai",
            "Laporan kegiatan bulanan", "Koordinasi program kesehatan",
            "Permintaan klarifikasi anggaran", "Undangan sosialisasi",
            "Pengajuan kerja sama", "Pemberitahuan kegiatan pemerintahan"
        };

        var suratMasukList = Enumerable.Range(1, 25).Select(i =>
        {
            var tgl = new DateOnly(2025, 1, 1).AddDays(i);
            return new SuratMasuk
            {
                NoSurat      = $"SM/{i:000}/I/2025",
                NomorAgenda  = $"AGENDA/{i:000}/I/2025",
                TanggalSurat = tgl,
                AsalPengirim = pengirimList[i % pengirimList.Length],
                Perihal      = perihalList[i % perihalList.Length],
                NamaFile     = i % 3 == 0 ? $"lampiran_{i}.pdf" : null,
                IsArchived   = i % 5 == 0,
                CreatedAt    = DateTime.UtcNow
            };
        }).ToList();

        await db.SuratMasuk.AddRangeAsync(suratMasukList);

        var suratKeluarList = Enumerable.Range(1, 15).Select(i =>
        {
            var tgl = new DateOnly(2025, 1, 1).AddDays(i * 2);
            return new SuratKeluar
            {
                NoSurat      = i % 3 == 0 ? $"SK/{i:000}/I/2025" : null,
                NomorAgenda  = $"AGENDA-K/{i:000}/I/2025",
                TanggalSurat = tgl,
                Kepada       = pengirimList[i % pengirimList.Length],
                Perihal      = perihalList[i % perihalList.Length],
                NamaFile     = i % 4 == 0 ? $"surat_keluar_{i}.pdf" : null,
                Status       = i % 4 == 0 ? "Terkirim" : i % 3 == 0 ? "Disetujui" : "Draft",
                IsArchived   = i % 6 == 0,
                CreatedAt    = DateTime.UtcNow
            };
        }).ToList();

        await db.SuratKeluar.AddRangeAsync(suratKeluarList);
        await db.SaveChangesAsync();
    }
}