using MailDesk.API.Data;
using MailDesk.API.DTOs.SuratMasuk;
using MailDesk.API.Entities;
using MailDesk.API.Helpers;
using MailDesk.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailDesk.API.Services;

public class SuratMasukService : ISuratMasukService
{
    private readonly AppDbContext _context;

    public SuratMasukService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SuratMasukResponse> CreateSuratMasukAsync(CreateSuratMasukRequest request)
    {
        // 1. Validasi: apakah no_surat sudah pernah dicatat?
        var isDuplicate = await _context.SuratMasuks
            .AnyAsync(s => s.NoSurat == request.NoSurat);

        if (isDuplicate)
            throw new InvalidOperationException(
                $"Surat dengan nomor '{request.NoSurat}' sudah pernah dicatat.");

        // 2. Generate nomor agenda otomatis
        var now = DateTime.Now;
        var jumlahBulanIni = await _context.SuratMasuks
            .CountAsync(s => s.CreatedAt.Month == now.Month 
                          && s.CreatedAt.Year == now.Year);

        var nomorAgenda = NomorAgendaHelper.Generate(jumlahBulanIni + 1);

        // 3. Buat entitas baru
        var suratMasuk = new SuratMasuk
        {
            NoSurat      = request.NoSurat,
            NomorAgenda  = nomorAgenda,
            TanggalSurat = request.TanggalSurat,
            AsalPengirim = request.AsalPengirim,
            Perihal      = request.Perihal,
            UserId       = request.UserId,
            CreatedAt    = DateTime.UtcNow,
            IsArchived   = false
        };

        // 4. Simpan ke database
        _context.SuratMasuks.Add(suratMasuk);
        await _context.SaveChangesAsync();

        // 5. Load relasi user untuk response
        await _context.Entry(suratMasuk)
            .Reference(s => s.User)
            .LoadAsync();

        // 6. Map ke response DTO
        return MapToResponse(suratMasuk);
    }

    private static SuratMasukResponse MapToResponse(SuratMasuk s)
    {
        return new SuratMasukResponse
        {
            Id           = s.Id,
            NoSurat      = s.NoSurat,
            NomorAgenda  = s.NomorAgenda ?? "-",
            TanggalSurat = s.TanggalSurat,
            AsalPengirim = s.AsalPengirim,
            Perihal      = s.Perihal,
            IsArchived   = s.IsArchived,
            PencatatNama = s.User?.Nama,
            CreatedAt    = s.CreatedAt
        };
    }
}