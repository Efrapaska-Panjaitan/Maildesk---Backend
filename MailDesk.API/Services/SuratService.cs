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

    public SuratService(
        AppDbContext context,
        ILogger<SuratService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SuratResponse> CreateSuratMasukAsync(CreateSuratRequest request)
    {
        // Validasi duplikat no_surat
        var isDuplicate = await _context.Surats
            .AnyAsync(s => s.NoSurat == request.NoSurat
                        && s.JenisSurat == "Masuk");

        if (isDuplicate)
            throw new InvalidOperationException(
                $"Surat dengan nomor '{request.NoSurat}' sudah pernah dicatat.");

        // ── Generate nomor agenda ─────────────────────────────────
        string nomorAgenda;
        var now = DateTime.Now;

        // Jika frontend kirim preview dan belum dipakai, gunakan itu
        if (!string.IsNullOrEmpty(request.NomorAgendaPreview))
        {
            var isPreviewSudahDipakai = await _context.Surats
                .AnyAsync(s => s.NomorAgenda == request.NomorAgendaPreview);

            if (!isPreviewSudahDipakai)
            {
                // Preview masih available → gunakan nomor agenda dari preview
                nomorAgenda = request.NomorAgendaPreview;
                _logger.LogInformation(
                    "Menggunakan nomor agenda dari preview: {NomorAgenda}", nomorAgenda);
            }
            else
            {
                // Preview sudah dipakai orang lain → generate nomor baru
                var jumlahBulanIni = await _context.Surats
                    .CountAsync(s => s.CreatedAt.Month == now.Month
                                && s.CreatedAt.Year == now.Year);

                nomorAgenda = NomorAgendaHelper.Generate(jumlahBulanIni + 1);

                _logger.LogWarning(
                    "Preview {Preview} sudah dipakai. Nomor baru: {NomorAgenda}",
                    request.NomorAgendaPreview, nomorAgenda);
            }
        }
        else
        {
            // Frontend tidak kirim preview → generate seperti biasa
            var jumlahBulanIni = await _context.Surats
                .CountAsync(s => s.CreatedAt.Month == now.Month
                            && s.CreatedAt.Year == now.Year);

            nomorAgenda = NomorAgendaHelper.Generate(jumlahBulanIni + 1);
        }

        // Buat entitas baru
        var surat = new Surat
        {
            NoSurat      = request.NoSurat,
            NomorAgenda  = nomorAgenda,
            JenisSurat   = "Masuk",                // ← Fix ke 'Masuk'
            KategoriSurat = request.KategoriSurat,
            TanggalSurat = request.TanggalSurat,
            Pengirim     = request.Pengirim,
            Penerima     = request.Penerima,
            Perihal      = request.Perihal,
            Status       = "Baru",
            UserId       = request.UserId,
            CreatedAt    = DateTime.UtcNow,
            IsArchived   = false
        };

        _context.Surats.Add(surat);
        await _context.SaveChangesAsync();

        await _context.Entry(surat)
            .Reference(s => s.User)
            .LoadAsync();

        _logger.LogInformation(
            "Surat masuk dicatat. NomorAgenda: {NomorAgenda}, NoSurat: {NoSurat}",
            nomorAgenda, request.NoSurat);

        return MapToResponse(surat);
    }

    public async Task<UploadPdfResponse> UploadPdfAsync(int suratId, IFormFile file)
    {
        // ── Validasi 1: Surat masuk harus ada ─────────────────────
        var surat = await _context.Surats.FindAsync(suratId);
        if (surat == null)
            throw new KeyNotFoundException($"Surat dengan ID {suratId} tidak ditemukan.");

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
        var subFolder = Path.Combine("wwwroot", "uploads", "surat",
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

        // ── Update database ────────────────────────────────────────
        surat.NamaFile = file.FileName;       // nama asli file

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "PDF berhasil diupload. SuratId: {Id}, File: {NamaFile}",
            suratId, file.FileName);

        return new UploadPdfResponse
        {
            SuratId       = suratId,
            NamaFile      = file.FileName,
            FileSizeBytes = file.Length,
            UploadedAt    = DateTime.UtcNow
        };
    }

    public async Task<NomorAgendaPreviewResponse> GetNomorAgendaPreviewAsync()
    {
        // Hitung jumlah surat masuk bulan ini
        var now = DateTime.Now;
        var jumlahBulanIni = await _context.Surats
            .CountAsync(s => s.JenisSurat == "Masuk"
                          && s.CreatedAt.Month == now.Month
                          && s.CreatedAt.Year == now.Year);

        // Generate preview nomor agenda berikutnya
        var preview = NomorAgendaHelper.Preview(jumlahBulanIni);

        _logger.LogInformation(
            "Nomor agenda preview generated: {NomorAgenda}", preview);

        return new NomorAgendaPreviewResponse
        {
            NomorAgenda  = preview,
            Keterangan   = "Preview - nomor final dikonfirmasi saat simpan",
            GeneratedAt  = DateTime.UtcNow
        };
    }

    public async Task<SuratResponse> GetSuratByIdAsync(int id)
    {
        var surat = await _context.Surats
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (surat == null)
            throw new KeyNotFoundException($"Surat dengan ID {id} tidak ditemukan.");

        return MapToResponse(surat);
    }

    private static SuratResponse MapToResponse(Surat s)
    {
        return new SuratResponse
        {
            Id           = s.Id,
            NoSurat      = s.NoSurat,
            NomorAgenda  = s.NomorAgenda ?? "-",
            JenisSurat   = s.JenisSurat,
            KategoriSurat = s.KategoriSurat,
            TanggalSurat = s.TanggalSurat,
            Pengirim     = s.Pengirim,
            Penerima     = s.Penerima,
            Perihal      = s.Perihal,
            Status       = s.Status,
            IsArchived   = s.IsArchived,
            PencatatNama = s.User?.Nama,
            NamaFile     = s.NamaFile,
            CreatedAt    = s.CreatedAt
        };
    }

    // GET ALL SURAT (Dashboard/Inbox)
    public async Task<PaginatedResponse<SuratListResponse>> GetAllSuratAsync(SuratQueryParams query)
    {
        return await GetSuratByFilterAsync(query, jenisSurat: null);
    }

    // GET SURAT MASUK
    public async Task<PaginatedResponse<SuratListResponse>> GetSuratMasukAsync(SuratQueryParams query)
    {
        return await GetSuratByFilterAsync(query, jenisSurat: "Masuk");
    }

    // GET SURAT KELUAR
    public async Task<PaginatedResponse<SuratListResponse>> GetSuratKeluarAsync(SuratQueryParams query)
    {
        return await GetSuratByFilterAsync(query, jenisSurat: "Keluar");
    }

    // CORE METHOD — Reusable filter logic
    private async Task<PaginatedResponse<SuratListResponse>> GetSuratByFilterAsync(SuratQueryParams query, string? jenisSurat)
    {
        // ── Base query ──────────────────────────────────────────
        var q = _context.Surats
            .Include(s => s.User)
            .AsQueryable();

        // ── Filter jenis surat ──────────────────────────────────
        if (!string.IsNullOrEmpty(jenisSurat))
            q = q.Where(s => s.JenisSurat == jenisSurat);

        // ── Filter arsip ────────────────────────────────────────
        if (!query.IncludeArchived)
            q = q.Where(s => s.IsArchived == false);

        // ── Filter status ───────────────────────────────────────
        if (!string.IsNullOrEmpty(query.Status))
            q = q.Where(s => s.Status == query.Status);

        // ── Filter kategori ─────────────────────────────────────
        if (!string.IsNullOrEmpty(query.KategoriSurat))
            q = q.Where(s => s.KategoriSurat == query.KategoriSurat);

        // ── Filter tanggal ──────────────────────────────────────
        if (query.TanggalDari.HasValue)
            q = q.Where(s => s.TanggalSurat >= query.TanggalDari.Value);

        if (query.TanggalSampai.HasValue)
            q = q.Where(s => s.TanggalSurat <= query.TanggalSampai.Value);

        // ── Search (perihal / pengirim / no_surat) ──────────────
        if (!string.IsNullOrEmpty(query.Search))
        {
            var keyword = query.Search.ToLower();
            q = q.Where(s =>
                s.Perihal.ToLower().Contains(keyword) ||
                s.Pengirim.ToLower().Contains(keyword) ||
                s.Penerima.ToLower().Contains(keyword) ||
                (s.NoSurat != null && s.NoSurat.ToLower().Contains(keyword)) ||
                (s.NomorAgenda != null && s.NomorAgenda.ToLower().Contains(keyword)));
        }

        // ── Sorting ─────────────────────────────────────────────
        var isAsc = query.SortOrder.ToLower() == "asc";

        q = query.SortBy.ToLower() switch
        {
            "tanggal"       => isAsc ? q.OrderBy(s => s.TanggalSurat)
                                    : q.OrderByDescending(s => s.TanggalSurat),
            "nomor_agenda"  => isAsc ? q.OrderBy(s => s.NomorAgenda)
                                    : q.OrderByDescending(s => s.NomorAgenda),
            "pengirim"      => isAsc ? q.OrderBy(s => s.Pengirim)
                                    : q.OrderByDescending(s => s.Pengirim),
            "status"        => isAsc ? q.OrderBy(s => s.Status)
                                    : q.OrderByDescending(s => s.Status),
            _               => q.OrderByDescending(s => s.CreatedAt) // default
        };

        // ── Pagination ──────────────────────────────────────────
        var totalData = await q.CountAsync();

        var limit = Math.Max(1, Math.Min(query.Limit, 100)); // max 100 per page
        var page  = Math.Max(1, query.Page);
        var skip  = (page - 1) * limit;

        var data = await q
            .Skip(skip)
            .Take(limit)
            .Select(s => new SuratListResponse
            {
                Id            = s.Id,
                NoSurat       = s.NoSurat,
                NomorAgenda   = s.NomorAgenda ?? "-",
                JenisSurat    = s.JenisSurat,
                KategoriSurat = s.KategoriSurat,
                TanggalSurat  = s.TanggalSurat,
                Pengirim      = s.Pengirim,
                Penerima      = s.Penerima,
                Perihal       = s.Perihal,
                Status        = s.Status,
                HasLampiran   = s.NamaFile != null,
                CreatedAt     = s.CreatedAt
            })
            .ToListAsync();

        var totalPages = (int)Math.Ceiling((double)totalData / limit);

        return new PaginatedResponse<SuratListResponse>
        {
            Data = data,
            Meta = new PaginationMeta
            {
                CurrentPage = page,
                TotalPages  = totalPages,
                TotalData   = totalData,
                Limit       = limit,
                HasNextPage = page < totalPages,
                HasPrevPage = page > 1
            }
        };
    }
}