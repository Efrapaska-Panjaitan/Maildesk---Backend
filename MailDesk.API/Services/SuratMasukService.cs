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
    private readonly ILogger<SuratMasukService> _logger;

    public SuratMasukService(
        AppDbContext context,
        ILogger<SuratMasukService> logger)
    {
        _context = context;
        _logger = logger;
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

        _logger.LogInformation(
            "Surat masuk berhasil dicatat. NomorAgenda: {NomorAgenda}, NoSurat: {NoSurat}",
            nomorAgenda, request.NoSurat);

        // 6. Map ke response DTO
        return MapToResponse(suratMasuk);
    }

    public async Task<UploadPdfResponse> UploadPdfAsync(int suratMasukId, IFormFile file)
    {
        // ── Validasi 1: Surat masuk harus ada ─────────────────────
        var suratMasuk = await _context.SuratMasuks.FindAsync(suratMasukId);
        if (suratMasuk == null)
            throw new KeyNotFoundException($"Surat masuk dengan ID {suratMasukId} tidak ditemukan.");

        // ── Validasi 2: File tidak boleh kosong ───────────────────
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("File tidak boleh kosong.");

        // ── Validasi 3: Tipe file harus PDF ───────────────────────
        var allowedExtensions = new[] { ".pdf" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
            throw new InvalidOperationException(
                $"Tipe file tidak valid. Hanya file PDF yang diizinkan.");

        // Cek MIME type juga (double validation)
        var allowedMimeTypes = new[] { "application/pdf" };
        if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            throw new InvalidOperationException(
                "MIME type tidak valid. Hanya application/pdf yang diizinkan.");

        // ── Validasi 4: Ukuran file maksimal 10MB ─────────────────
        const long maxFileSizeBytes = 10 * 1024 * 1024; // 10MB
        if (file.Length > maxFileSizeBytes)
            throw new InvalidOperationException(
                $"Ukuran file terlalu besar. Maksimal 10MB, file kamu: " +
                $"{file.Length / (1024 * 1024)}MB.");

        // ── Simpan file ke folder uploads ─────────────────────────
        // Generate unique filename agar tidak bentrok
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

        // Folder: wwwroot/uploads/surat-masuk/2026/05/
        var now = DateTime.Now;
        var subFolder = Path.Combine("wwwroot", "uploads", "surat-masuk",
            now.Year.ToString(), now.Month.ToString("D2"));

        // Buat folder kalau belum ada
        Directory.CreateDirectory(subFolder);

        // Full path di server
        var fullPath = Path.Combine(subFolder, uniqueFileName);

        // Simpan file ke disk
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Path yang disimpan ke DB (relative path)
        var relativePath = Path.Combine("uploads", "surat-masuk",
            now.Year.ToString(), now.Month.ToString("D2"), uniqueFileName);

        // ── Update database ────────────────────────────────────────
        suratMasuk.NamaFile = file.FileName;       // nama asli file
        suratMasuk.FilePath = relativePath;         // path di server

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "PDF berhasil diupload. SuratMasukId: {Id}, File: {NamaFile}, Path: {FilePath}",
            suratMasukId, file.FileName, relativePath);

        return new UploadPdfResponse
        {
            SuratMasukId  = suratMasukId,
            NamaFile      = file.FileName,
            FilePath      = relativePath,
            FileSizeBytes = file.Length,
            UploadedAt    = DateTime.UtcNow
        };
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
            NamaFile     = s.NamaFile,
            FilePath     = s.FilePath,
            CreatedAt    = s.CreatedAt
        };
    }
}