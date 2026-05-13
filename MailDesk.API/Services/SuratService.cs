using MailDesk.API.Data;
using MailDesk.API.DTOs.Surat;
using MailDesk.API.Entities;
using MailDesk.API.Helpers;
using MailDesk.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailDesk.API.Services;

public class SuratService : ISuratService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SuratService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SuratService(
        AppDbContext context,
        ILogger<SuratService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    // ─────────────────────────────────────────────────────────
    // CREATE SURAT MASUK (termasuk upload PDF — wajib)
    // ─────────────────────────────────────────────────────────
    public async Task<SuratResponse> CreateSuratMasukAsync(CreateSuratRequest request, IFormFile file)
    {
        // ── Validasi duplikat no_surat ────────────────────────
        var isDuplicate = await _context.Surats
            .AnyAsync(s => s.NoSurat == request.NoSurat && s.JenisSurat == "Masuk");

        if (isDuplicate)
            throw new InvalidOperationException(
                $"Surat dengan nomor '{request.NoSurat}' sudah pernah dicatat.");

        // ── Validasi DitujukanKe ──────────────────────────────
        var ditujukanKeExists = await _context.Users
            .AnyAsync(u => u.Id == request.DitujukanKeId);
        if (!ditujukanKeExists)
            throw new KeyNotFoundException(
                $"User dengan ID {request.DitujukanKeId} (penerima tujuan) tidak ditemukan.");

        // ── Generate nomor agenda ─────────────────────────────
        string nomorAgenda;
        var now = DateTime.Now;

        if (!string.IsNullOrEmpty(request.NomorAgendaPreview)
            && NomorAgendaHelper.IsValidFormat(request.NomorAgendaPreview))
        {
            var isPreviewSudahDipakai = await _context.Surats
                .AnyAsync(s => s.NomorAgenda == request.NomorAgendaPreview);

            if (!isPreviewSudahDipakai)
            {
                nomorAgenda = request.NomorAgendaPreview;
                _logger.LogInformation("Menggunakan nomor agenda dari preview: {NomorAgenda}", nomorAgenda);
            }
            else
            {
                var jumlah = await _context.Surats
                    .CountAsync(s => s.JenisSurat == "Masuk"
                                  && s.CreatedAt.Month == now.Month
                                  && s.CreatedAt.Year == now.Year);
                nomorAgenda = NomorAgendaHelper.Generate("Masuk", jumlah + 1);
                _logger.LogWarning("Preview {Preview} sudah dipakai. Nomor baru: {NomorAgenda}",
                    request.NomorAgendaPreview, nomorAgenda);
            }
        }
        else
        {
            var jumlah = await _context.Surats
                .CountAsync(s => s.JenisSurat == "Masuk"
                              && s.CreatedAt.Month == now.Month
                              && s.CreatedAt.Year == now.Year);
            nomorAgenda = NomorAgendaHelper.Generate("Masuk", jumlah + 1);
        }

        // ── Simpan file PDF ───────────────────────────────────
        var (namaFile, filePath) = await SavePdfToDiskAsync(file);

        // ── Buat entitas surat ────────────────────────────────
        var surat = new Surat
        {
            NoSurat       = request.NoSurat,
            NomorAgenda   = nomorAgenda,
            JenisSurat    = "Masuk",
            KategoriSurat = request.KategoriSurat,
            TanggalSurat  = request.TanggalSurat,
            Pengirim      = request.Pengirim,
            Penerima      = request.Penerima,
            Perihal       = request.Perihal,
            Status        = "Baru",
            UserId        = request.UserId,
            DitujukanKeId = request.DitujukanKeId,
            NamaFile      = namaFile,   // nama asli dari pengirim
            FilePath      = filePath,   // path relatif di disk
            CreatedAt     = DateTime.Now,
            IsArchived    = false
        };

        _context.Surats.Add(surat);
        await _context.SaveChangesAsync();

        // Load navigation properties
        await _context.Entry(surat).Reference(s => s.User).LoadAsync();
        await _context.Entry(surat).Reference(s => s.DitujukanKe).LoadAsync();

        _logger.LogInformation(
            "Surat masuk dicatat. NomorAgenda: {NomorAgenda}, File: {NamaFile}",
            nomorAgenda, namaFile);

        return MapToResponse(surat);
    }

    // ─────────────────────────────────────────────────────────
    // UPLOAD PDF (ganti/replace file yang sudah ada)
    // ─────────────────────────────────────────────────────────
    public async Task<UploadPdfResponse> UploadPdfAsync(int suratId, IFormFile file)
    {
        var surat = await _context.Surats.FindAsync(suratId)
            ?? throw new KeyNotFoundException($"Surat dengan ID {suratId} tidak ditemukan.");

        // Hapus file lama dari disk jika ada
        if (!string.IsNullOrEmpty(surat.FilePath))
        {
            var oldFullPath = Path.Combine("wwwroot", surat.FilePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(oldFullPath)) File.Delete(oldFullPath);
        }

        var (namaFile, filePath) = await SavePdfToDiskAsync(file);

        surat.NamaFile = namaFile;
        surat.FilePath = filePath;
        await _context.SaveChangesAsync();

        var req     = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = req != null ? $"{req.Scheme}://{req.Host}" : string.Empty;

        _logger.LogInformation("PDF di-replace. SuratId: {Id}, File: {NamaFile}", suratId, namaFile);

        return new UploadPdfResponse
        {
            SuratId       = suratId,
            NamaFileAsli  = namaFile,
            FilePath      = filePath,
            FileUrl       = $"{baseUrl}/{filePath}",
            FileSizeBytes = file.Length,
            UploadedAt    = DateTime.Now
        };
    }

    // ─────────────────────────────────────────────────────────
    // GET NOMOR AGENDA PREVIEW
    // ─────────────────────────────────────────────────────────
    public async Task<NomorAgendaPreviewResponse> GetNomorAgendaPreviewAsync()
    {
        var now = DateTime.Now;
        var jumlahBulanIni = await _context.Surats
            .CountAsync(s => s.JenisSurat == "Masuk"
                          && s.CreatedAt.Month == now.Month
                          && s.CreatedAt.Year == now.Year);

        var preview = NomorAgendaHelper.Preview(jumlahBulanIni);

        _logger.LogInformation("Nomor agenda preview generated: {NomorAgenda}", preview);

        return new NomorAgendaPreviewResponse
        {
            NomorAgenda = preview,
            Keterangan  = "Preview - nomor final dikonfirmasi saat simpan",
            GeneratedAt = DateTime.Now
        };
    }

    // ─────────────────────────────────────────────────────────
    // GET SURAT BY ID
    // ─────────────────────────────────────────────────────────
    public async Task<SuratResponse> GetSuratByIdAsync(int id)
    {
        var surat = await _context.Surats
            .Include(s => s.User)
            .Include(s => s.DitujukanKe)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (surat == null)
            throw new KeyNotFoundException($"Surat dengan ID {id} tidak ditemukan.");

        return MapToResponse(surat);
    }

    // ─────────────────────────────────────────────────────────
    // GET ALL SURAT MASUK (tanpa filter — semua data)
    // ─────────────────────────────────────────────────────────
    public async Task<IEnumerable<SuratListResponse>> GetAllSuratMasukAsync()
    {
        return await _context.Surats
            .Include(s => s.DitujukanKe)
            .Where(s => s.JenisSurat == "Masuk")
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SuratListResponse
            {
                Id              = s.Id,
                NoSurat         = s.NoSurat,
                NomorAgenda     = s.NomorAgenda ?? "-",
                JenisSurat      = s.JenisSurat,
                KategoriSurat   = s.KategoriSurat,
                TanggalSurat    = s.TanggalSurat,
                Pengirim        = s.Pengirim,
                Penerima        = s.Penerima,
                Perihal         = s.Perihal,
                Status          = s.Status,
                HasLampiran     = s.FilePath != null,
                DitujukanKeNama = s.DitujukanKe != null ? s.DitujukanKe.Nama : null,
                CreatedAt       = s.CreatedAt
            })
            .ToListAsync();
    }

    // ─────────────────────────────────────────────────────────
    // PRIVATE HELPER — Validasi & simpan PDF ke disk
    // ─────────────────────────────────────────────────────────
    private static async Task<(string namaFile, string filePath)> SavePdfToDiskAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("File PDF wajib disertakan dan tidak boleh kosong.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext != ".pdf")
            throw new InvalidOperationException("Hanya file PDF yang diizinkan.");

        if (file.ContentType.ToLowerInvariant() != "application/pdf")
            throw new InvalidOperationException("MIME type tidak valid. Hanya application/pdf yang diizinkan.");

        const long maxSize = 10L * 1024 * 1024; // 10 MB
        if (file.Length > maxSize)
            throw new InvalidOperationException(
                $"Ukuran file terlalu besar. Maksimal 10MB, file kamu: {file.Length / (1024 * 1024)}MB.");

        // UUID filename agar tidak bentrok di disk
        var uniqueFileName = $"{Guid.NewGuid()}{ext}";
        var now = DateTime.Now;
        var yearMonth = Path.Combine(now.Year.ToString(), now.Month.ToString("D2"));
        var subFolder = Path.Combine("wwwroot", "uploads", "surat", yearMonth);
        Directory.CreateDirectory(subFolder);

        var fullPath = Path.Combine(subFolder, uniqueFileName);
        using (var stream = new FileStream(fullPath, FileMode.Create))
            await file.CopyToAsync(stream);

        // nama asli untuk display, path relatif untuk URL
        var namaFile = file.FileName;
        var filePath = $"uploads/surat/{yearMonth.Replace(Path.DirectorySeparatorChar, '/')}/{uniqueFileName}";

        return (namaFile, filePath);
    }

    // ─────────────────────────────────────────────────────────
    // PRIVATE HELPER — Map entity ke SuratResponse
    // ─────────────────────────────────────────────────────────
    private SuratResponse MapToResponse(Surat s)
    {
        var req     = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = req != null ? $"{req.Scheme}://{req.Host}" : string.Empty;

        return new SuratResponse
        {
            Id              = s.Id,
            NoSurat         = s.NoSurat,
            NomorAgenda     = s.NomorAgenda ?? "-",
            JenisSurat      = s.JenisSurat,
            KategoriSurat   = s.KategoriSurat,
            TanggalSurat    = s.TanggalSurat,
            Pengirim        = s.Pengirim,
            Penerima        = s.Penerima,
            Perihal         = s.Perihal,
            Status          = s.Status,
            IsArchived      = s.IsArchived,
            PencatatNama    = s.User?.Nama,
            DitujukanKeId   = s.DitujukanKeId,
            DitujukanKeNama = s.DitujukanKe?.Nama,
            NamaFile        = s.NamaFile,   // nama asli file
            FileUrl         = s.FilePath != null ? $"{baseUrl}/{s.FilePath}" : null,
            HasLampiran     = s.FilePath != null,
            CreatedAt       = s.CreatedAt
        };
    }
}