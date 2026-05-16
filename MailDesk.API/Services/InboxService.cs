using MailDesk.API.Data;
using MailDesk.API.DTOs.Inbox;
using MailDesk.API.DTOs.Surat;
using MailDesk.API.Entities;
using MailDesk.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailDesk.API.Services;

public class InboxService : IInboxService
{
    private readonly AppDbContext _context;
    private readonly ILogger<InboxService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InboxService(
        AppDbContext context,
        ILogger<InboxService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    // ─────────────────────────────────────────────────────────
    // GET ALL INBOX dengan filter, search, sorting, pagination
    // ─────────────────────────────────────────────────────────
    public async Task<PaginatedResponse<InboxListResponse>> GetAllInboxAsync(InboxQueryParams query)
    {
        var q = _context.Inboxes
            .Include(i => i.Surat)
            .Include(i => i.Pengirim)
            .Include(i => i.Penerima)
            .AsQueryable();

        // Filter penerima — jika FilterByPenerimaId diisi, hanya tampilkan inbox untuk user itu
        if (query.FilterByPenerimaId.HasValue)
            q = q.Where(i => i.PenerimaId == query.FilterByPenerimaId.Value);

        // Filter status
        if (!string.IsNullOrEmpty(query.Status))
            q = q.Where(i => i.Status == query.Status);

        // Search (nomor agenda / perihal / pengirim)
        if (!string.IsNullOrEmpty(query.Search))
        {
            var keyword = query.Search.ToLower();
            q = q.Where(i =>
                (i.Surat != null && !string.IsNullOrEmpty(i.Surat.NomorAgenda) && i.Surat.NomorAgenda.ToLower().Contains(keyword)) ||
                (i.Surat != null && !string.IsNullOrEmpty(i.Surat.Perihal) && i.Surat.Perihal.ToLower().Contains(keyword)) ||
                (i.Surat != null && !string.IsNullOrEmpty(i.Surat.Pengirim) && i.Surat.Pengirim.ToLower().Contains(keyword)));
        }

        // Filter tanggal
        if (query.TanggalDari.HasValue)
            q = q.Where(i => i.CreatedAt.Date >= query.TanggalDari.Value.ToDateTime(TimeOnly.MinValue));

        if (query.TanggalSampai.HasValue)
            q = q.Where(i => i.CreatedAt.Date <= query.TanggalSampai.Value.ToDateTime(TimeOnly.MaxValue));

        // Sorting
        q = query.SortBy?.ToLower() switch
        {
            "tanggal" => query.SortOrder == "asc"
                ? q.OrderBy(i => i.CreatedAt)
                : q.OrderByDescending(i => i.CreatedAt),
            "pengirim" => query.SortOrder == "asc"
                ? q.OrderBy(i => i.Surat != null ? i.Surat.Pengirim : "")
                : q.OrderByDescending(i => i.Surat != null ? i.Surat.Pengirim : ""),
            "status" => query.SortOrder == "asc"
                ? q.OrderBy(i => i.Status)
                : q.OrderByDescending(i => i.Status),
            _ => query.SortOrder == "asc"
                ? q.OrderBy(i => i.CreatedAt)
                : q.OrderByDescending(i => i.CreatedAt)
        };

        // Pagination
        int totalData = await q.CountAsync();
        int totalPages = (int)Math.Ceiling(totalData / (double)query.Limit);
        int skip = (query.Page - 1) * query.Limit;

        var data = await q
            .Skip(skip)
            .Take(query.Limit)
            .Select(i => new InboxListResponse
            {
                Id = i.Id,
                SuratId = i.SuratId,
                NomorAgenda = i.Surat != null ? i.Surat.NomorAgenda : null,
                NoSurat = i.Surat != null ? i.Surat.NoSurat : null,
                Pengirim = i.Surat != null ? i.Surat.Pengirim : null,
                Perihal = i.Surat != null ? i.Surat.Perihal : null,
                Status = i.Status,
                CatatanPengantar = i.CatatanPengantar,
                NamaFile = i.Surat != null ? i.Surat.NamaFile : null,
                CreatedAt = i.CreatedAt
            })
            .ToListAsync();

        _logger.LogInformation("Get all inbox. Total: {Total}, Page: {Page}, FilterByPenerima: {FilterByPenerima}",
            totalData, query.Page, query.FilterByPenerimaId ?? -1);

        return new PaginatedResponse<InboxListResponse>
        {
            Data = data,
            Meta = new PaginationMeta
            {
                CurrentPage = query.Page,
                TotalPages = totalPages,
                TotalData = totalData,
                Limit = query.Limit,
                HasNextPage = query.Page < totalPages,
                HasPrevPage = query.Page > 1
            }
        };
    }

    // ─────────────────────────────────────────────────────────
    // GET INBOX BY ID
    // ─────────────────────────────────────────────────────────
    public async Task<InboxDetailResponse> GetInboxByIdAsync(int id)
    {
        var inbox = await _context.Inboxes
            .Include(i => i.Surat)
            .Include(i => i.Pengirim)
            .Include(i => i.Penerima)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (inbox == null)
            throw new KeyNotFoundException($"Inbox dengan ID {id} tidak ditemukan.");

        var req = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = req != null ? $"{req.Scheme}://{req.Host}" : string.Empty;

        _logger.LogInformation("Get inbox by ID: {Id}", id);

        return new InboxDetailResponse
        {
            Id = inbox.Id,
            SuratId = inbox.SuratId,
            PengirimId = inbox.PengirimId,
            PenerimaId = inbox.PenerimaId,
            NomorAgenda = inbox.Surat?.NomorAgenda,
            NoSurat = inbox.Surat?.NoSurat,
            Pengirim = inbox.Surat?.Pengirim,
            PengirimName = inbox.Surat?.Pengirim,
            PenerimaName = inbox.Penerima?.Nama,
            Perihal = inbox.Surat?.Perihal,
            KategoriSurat = inbox.Surat?.KategoriSurat,
            TanggalSurat = inbox.Surat != null ? inbox.Surat.TanggalSurat : null,
            Status = inbox.Status,
            CatatanPengantar = inbox.CatatanPengantar,
            NamaFile = inbox.Surat?.NamaFile,
            FileUrl = !string.IsNullOrEmpty(inbox.Surat?.FilePath)
                ? $"{baseUrl}/{inbox.Surat.FilePath}"
                : null,
            CreatedAt = inbox.CreatedAt
        };
    }

    // ─────────────────────────────────────────────────────────
    // CREATE INBOX FROM SURAT (multi-penerima, maks 5)
    // ─────────────────────────────────────────────────────────
    public async Task<List<InboxDetailResponse>> CreateInboxFromSuratAsync(CreateInboxFromSuratRequest request)
    {
        // Validasi duplikat dalam request itu sendiri
        var distinctIds = request.PenerimaIds.Distinct().ToList();
        if (distinctIds.Count != request.PenerimaIds.Count)
            throw new ArgumentException("Terdapat ID penerima yang duplikat dalam request.");

        // Validasi surat
        var surat = await _context.Surats
            .FirstOrDefaultAsync(s => s.Id == request.SuratId);

        if (surat == null)
            throw new KeyNotFoundException(
                $"Surat dengan ID {request.SuratId} tidak ditemukan.");

        // Validasi semua penerima ada di tabel users
        var validUserIds = await _context.Users
            .Where(u => distinctIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync();

        var invalidIds = distinctIds.Except(validUserIds).ToList();
        if (invalidIds.Any())
            throw new KeyNotFoundException(
                $"User penerima dengan ID berikut tidak ditemukan: {string.Join(", ", invalidIds)}.");

        // Cek apakah surat ini sudah pernah diteruskan ke user-user tersebut
        var existingPenerimaIds = await _context.Inboxes
            .Where(i => i.SuratId == request.SuratId && distinctIds.Contains(i.PenerimaId!.Value))
            .Select(i => i.PenerimaId!.Value)
            .ToListAsync();

        if (existingPenerimaIds.Any())
            throw new InvalidOperationException(
                $"Surat ini sudah pernah diteruskan ke user dengan ID: {string.Join(", ", existingPenerimaIds)}.");

        // Buat inbox entry untuk setiap penerima
        var inboxEntries = distinctIds.Select(penerimaId => new Inbox
        {
            SuratId        = request.SuratId,
            PengirimId     = surat.UserId,
            PenerimaId     = penerimaId,
            Status         = "Menunggu Tindakan",
            CatatanPengantar = request.CatatanPengantar,
            CreatedAt      = DateTime.UtcNow
        }).ToList();

        _context.Inboxes.AddRange(inboxEntries);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Inbox dibuat dari surat. SuratId: {SuratId}, Penerima: [{PenerimaIds}], InboxIds: [{InboxIds}]",
            request.SuratId,
            string.Join(", ", distinctIds),
            string.Join(", ", inboxEntries.Select(i => i.Id)));

        // Ambil detail setiap inbox yang baru dibuat
        var results = new List<InboxDetailResponse>();
        foreach (var inbox in inboxEntries)
            results.Add(await GetInboxByIdAsync(inbox.Id));

        return results;
    }
}
