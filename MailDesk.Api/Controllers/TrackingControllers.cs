using MailDesk.API.Data;
using MailDesk.API.DTOs;
using MailDesk.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrackingController : ControllerBase
{
    private readonly AppDbContext _context;

    public TrackingController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTrackingSurat(int id)
    {
        // 1. AMBIL DATA SURAT
        var surat = await _context.Surats
            .Include(s => s.User) 
            .FirstOrDefaultAsync(s => s.Id == id);

        if (surat == null)
        {
            return NotFound(new { message = "Surat tidak ditemukan!" });
        }

        // 2. SIAPKAN WADAH KOSONG
        var daftarRiwayat = new List<RiwayatItemDto>();

        // 3. RIWAYAT PERTAMA (DIBUAT)
        daftarRiwayat.Add(new RiwayatItemDto
        {
            Tanggal = surat.CreatedAt,
            JenisAksi = "Dibuat",
            Keterangan = "Surat baru didaftarkan",
            Dari = surat.User?.Nama ?? "Sistem",
            Kepada = "-" 
        });

        // 4. CARI DI INBOX
        var inboxes = await _context.Inboxes
            .Include(i => i.Pengirim)
            .Include(i => i.Penerima)
            .Where(i => i.SuratId == id)
            .ToListAsync();

        foreach (var inbox in inboxes)
        {
            daftarRiwayat.Add(new RiwayatItemDto
            {
                Tanggal = inbox.CreatedAt,
                JenisAksi = "Masuk Inbox",
                Keterangan = inbox.CatatanPengantar ?? "Dikirim ke Inbox",
                Dari = inbox.Pengirim?.Nama ?? "-",
                Kepada = inbox.Penerima?.Nama ?? "-"
            });
        }

        // 5. CARI DI DISPOSISI
        var disposisis = await _context.Disposisis
            .Include(d => d.Pemberi)
            .Include(d => d.Penerima)
            .Where(d => d.SuratId == id)
            .ToListAsync();

        foreach (var disposisi in disposisis)
        {
            daftarRiwayat.Add(new RiwayatItemDto
            {
                Tanggal = disposisi.CreatedAt,
                JenisAksi = "Disposisi",
                Keterangan = $"Instruksi: {disposisi.Instruksi}",
                Dari = disposisi.Pemberi?.Nama ?? "-",
                Kepada = disposisi.Penerima?.Nama ?? "-"
            });
        }

        // 6. URUTKAN WAKTU DARI YANG TERBARU
        daftarRiwayat = daftarRiwayat.OrderByDescending(r => r.Tanggal).ToList();

        // 7. BUNGKUS KE DTO
        var response = new TrackingResponseDto
        {
            IdSurat = surat.Id,
            NoSurat = surat.NoSurat,
            Perihal = surat.Perihal,
            StatusSurat = surat.Status,
            Timeline = daftarRiwayat
        };

        return Ok(new
        {
            status = "success",
            data = response
        });
    }
}