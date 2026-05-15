using MailDesk.API.Data;
using MailDesk.API.DTOs.Tracking;
using MailDesk.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailDesk.API.Services;

public class TrackingService : ITrackingService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TrackingService> _logger;

    public TrackingService(AppDbContext context, ILogger<TrackingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TrackingResponseDto> GetTrackingSuratAsync(int suratId)
    {
        // 1. Ambil data surat
        var surat = await _context.Surats
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == suratId);

        if (surat == null)
            throw new KeyNotFoundException($"Surat dengan ID {suratId} tidak ditemukan.");

        var daftarRiwayat = new List<RiwayatItemDto>();

        // 2. Riwayat pertama: surat dibuat
        daftarRiwayat.Add(new RiwayatItemDto
        {
            Tanggal    = surat.CreatedAt,
            JenisAksi  = "Dibuat",
            Keterangan = "Surat baru didaftarkan",
            Dari       = surat.User?.Nama ?? "Sistem",
            Kepada     = "-"
        });

        // 3. Riwayat dari tabel Inbox
        var inboxes = await _context.Inboxes
            .Include(i => i.Pengirim)
            .Include(i => i.Penerima)
            .Where(i => i.SuratId == suratId)
            .ToListAsync();

        foreach (var inbox in inboxes)
        {
            daftarRiwayat.Add(new RiwayatItemDto
            {
                Tanggal    = inbox.CreatedAt,
                JenisAksi  = "Masuk Inbox",
                Keterangan = inbox.CatatanPengantar ?? "Dikirim ke Inbox",
                Dari       = inbox.Pengirim?.Nama ?? "-",
                Kepada     = inbox.Penerima?.Nama ?? "-"
            });
        }

        // 4. Riwayat dari tabel Disposisi
        var disposisis = await _context.Disposisis
            .Include(d => d.Pemberi)
            .Include(d => d.Penerima)
            .Where(d => d.SuratId == suratId)
            .ToListAsync();

        foreach (var disposisi in disposisis)
        {
            daftarRiwayat.Add(new RiwayatItemDto
            {
                Tanggal    = disposisi.CreatedAt,
                JenisAksi  = "Disposisi",
                Keterangan = $"Instruksi: {disposisi.Instruksi}",
                Dari       = disposisi.Pemberi?.Nama ?? "-",
                Kepada     = disposisi.Penerima?.Nama ?? "-"
            });
        }

        // 5. Urutkan dari yang terbaru
        daftarRiwayat = daftarRiwayat.OrderByDescending(r => r.Tanggal).ToList();

        _logger.LogInformation("Get tracking surat ID: {SuratId}, total riwayat: {Total}", suratId, daftarRiwayat.Count);

        return new TrackingResponseDto
        {
            IdSurat    = surat.Id,
            NoSurat    = surat.NoSurat,
            Perihal    = surat.Perihal,
            StatusSurat = surat.Status,
            Timeline   = daftarRiwayat
        };
    }
}
