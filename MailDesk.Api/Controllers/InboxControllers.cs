using MailDesk.Api.Models;
using MailDesk.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InboxController : ControllerBase
{
    private readonly AppDbContext _context;

    public InboxController(AppDbContext context)
    {
        _context = context;
    }

    // Endpoint: GET api/inbox/{id_user_penerima}
    [HttpGet("{id_user}")]
    public async Task<IActionResult> GetInbox(int id_user)
    {
        // 1. CARI SEMUA SURAT DI INBOX MILIK USER INI
        var tumpukanInbox = await _context.Inboxes
            .Include(i => i.Surat)
            .Include(i => i.Pengirim) 
            .Where(i => i.PenerimaId == id_user)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        // 2. PINDAHKAN KE WADAH DTO
        var hasilRapi = new List<InboxResponseDto>();

        foreach (var item in tumpukanInbox)
        {
            hasilRapi.Add(new InboxResponseDto
            {
                IdInbox = item.Id,
                IdSurat = item.SuratId ?? 0,
                
                // Ambil data dari tabel Surat
                NoSurat = item.Surat?.NoSurat ?? "-",
                Perihal = item.Surat?.Perihal ?? "-",
                
                // Ambil nama dari tabel Users
                NamaPengirim = item.Pengirim?.Nama ?? "Sistem",
                
                WaktuMasuk = item.CreatedAt,
                Status = item.Status ?? "Belum Dibaca",
                CatatanPengantar = item.CatatanPengantar ?? "-"
            });
        }

        // 3. KIRIM KE FRONTEND
        return Ok(new
        {
            status = "success",
            jumlah_pesan = hasilRapi.Count,
            data = hasilRapi
        });
    }
}