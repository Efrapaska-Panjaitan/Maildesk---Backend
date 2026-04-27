using Maildesk.Api.Entities;
using Maildesk.Api.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Maildesk.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(MaildeskDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (await db.SuratMasuk.AnyAsync())
            return;

        var dummyData = new List<SuratMasuk>();

        var baseDate = new DateTime(2025, 1, 1);

        var pengirimList = new[]
        {
            "Akmal Fari",
            "Budi Santoso",
            "Sari Wulandari",
            "Dinas Kesehatan",
            "Kementerian Keuangan",
            "Bappeda Kota",
            "Dinas Pendidikan",
            "Sekretariat Daerah"
        };

        var instansiList = new[]
        {
            "Pemerintah Kota",
            "Kementerian Keuangan",
            "Dinas Kesehatan",
            "Dinas Pendidikan",
            "Bappeda",
            "Sekretariat Daerah"
        };

        var perihalList = new[]
        {
            "Undangan rapat pembangunan",
            "Permohonan data pegawai",
            "Laporan kegiatan bulanan",
            "Koordinasi program kesehatan",
            "Permintaan klarifikasi anggaran",
            "Undangan sosialisasi",
            "Pengajuan kerja sama",
            "Pemberitahuan kegiatan pemerintahan"
        };

        var statusList = new[]
        {
            "diterima",
            "dalam_disposisi",
            "didistribusikan",
            "selesai",
            "diarsipkan"
        };

        var jenisSumberList = new[]
        {
            "internal",
            "eksternal"
        };

        var kodeUnitList = new[]
        {
            "UMUM",
            "SDM",
            "KEU",
            "PEM"
        };

        for (int i = 1; i <= 25; i++)
        {
            var tanggal = baseDate.AddDays(i);
            var kodeUnit = kodeUnitList[i % kodeUnitList.Length];

            dummyData.Add(new SuratMasuk
            {
                NomorAgenda = NomorSuratHelper.GenerateNomorAgenda(i, tanggal),
                NomorSurat = NomorSuratHelper.GenerateNomorSurat(i, kodeUnit, tanggal),
                TanggalSurat = DateOnly.FromDateTime(tanggal),
                TanggalDiterima = DateOnly.FromDateTime(tanggal.AddDays(1)),
                NamaPengirim = pengirimList[i % pengirimList.Length],
                InstansiPengirim = instansiList[i % instansiList.Length],
                Perihal = perihalList[i % perihalList.Length],
                Deskripsi = $"Deskripsi surat ke-{i}",
                KodeKlasifikasi = $"KLS-{i:000}",
                JenisSumber = jenisSumberList[i % jenisSumberList.Length],
                TingkatPrioritas = i % 4 == 0 ? "tinggi" : "normal",
                Status = statusList[i % statusList.Length],
                DibuatPada = tanggal
            });
        }

        await db.SuratMasuk.AddRangeAsync(dummyData);
        await db.SaveChangesAsync();
    }
}