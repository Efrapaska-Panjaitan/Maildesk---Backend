using MailDesk.API.Data;
using MailDesk.API.DTOs.Disposisi;
using MailDesk.API.Entities;
using MailDesk.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailDesk.API.Services;

public class DisposisiService : IDisposisiService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DisposisiService> _logger;

    private static readonly string[] RoleBisaDisposisi = { "Admin", "Pimpinan" };

    public DisposisiService(AppDbContext context, ILogger<DisposisiService> logger)
    {
        _context = context;
        _logger  = logger;
    }

    // ─────────────────────────────────────────────────────────
    // CREATE DISPOSISI
    // ─────────────────────────────────────────────────────────
    public async Task<DisposisiDetailResponse> CreateDisposisiAsync(
        CreateDisposisiRequest request)
    {
        var surat = await _context.Surats.FindAsync(request.SuratId)
            ?? throw new KeyNotFoundException(
                $"Surat ID {request.SuratId} tidak ditemukan.");

        var pemberi = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == request.PemberiId)
            ?? throw new KeyNotFoundException(
                $"User pemberi ID {request.PemberiId} tidak ditemukan.");

        if (!RoleBisaDisposisi.Contains(pemberi.Role?.NamaRole))
            throw new UnauthorizedAccessException(
                $"Role '{pemberi.Role?.NamaRole}' tidak memiliki izin membuat disposisi.");

        var penerima = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == request.PenerimaId)
            ?? throw new KeyNotFoundException(
                $"User penerima ID {request.PenerimaId} tidak ditemukan.");

        if (request.PemberiId == request.PenerimaId)
            throw new InvalidOperationException(
                "Tidak bisa membuat disposisi ke diri sendiri.");

        var sifatValid = new[] { "Biasa", "Penting", "Mendesak", "Rahasia" };
        if (!sifatValid.Contains(request.SifatDisposisi))
            throw new InvalidOperationException(
                $"Sifat tidak valid. Pilihan: {string.Join(", ", sifatValid)}");

        var sudahAda = await _context.Disposisis
            .AnyAsync(d =>
                d.SuratId    == request.SuratId    &&
                d.PenerimaId == request.PenerimaId &&
                d.Status     != "Completed");

        if (sudahAda)
            throw new InvalidOperationException(
                $"Sudah ada disposisi aktif ke {penerima.Nama} untuk surat ini.");

        // Validasi parent jika ada
        if (request.ParentDisposisiId.HasValue)
        {
            var parent = await _context.Disposisis
                .FindAsync(request.ParentDisposisiId.Value)
                ?? throw new KeyNotFoundException(
                    $"Parent disposisi ID {request.ParentDisposisiId.Value} tidak ditemukan.");

            if (parent.SuratId != request.SuratId)
                throw new InvalidOperationException(
                    "Parent disposisi tidak terkait dengan surat yang sama.");
        }

        var disposisi = new Disposisi
        {
            SuratId          = request.SuratId,
            PemberiId        = request.PemberiId,
            PenerimaId       = request.PenerimaId,
            TanggalDisposisi = request.TanggalDisposisi,
            SifatDisposisi   = request.SifatDisposisi,
            Instruksi        = request.Instruksi,
            Status           = "Pending",
            CreatedAt        = DateTime.Now  // WIB
        };

        _context.Disposisis.Add(disposisi);
        await _context.SaveChangesAsync();

        // Simpan relasi chain
        if (request.ParentDisposisiId.HasValue)
        {
            _context.DisposisiRelations.Add(new DisposisiRelation
            {
                ParentId  = request.ParentDisposisiId.Value,
                ChildId   = disposisi.Id,
                CreatedAt = DateTime.Now  // WIB
            });
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation(
            "Disposisi dibuat. ID: {Id}, {Pemberi} → {Penerima}",
            disposisi.Id, pemberi.Nama, penerima.Nama);

        // Tulis log otomatis saat disposisi baru dibuat
        await AddLogAsync(
            disposisiId : disposisi.Id,
            suratId     : disposisi.SuratId,
            userId      : request.PemberiId,
            aksi        : "DIBUAT",
            statusLama  : null,
            statusBaru  : disposisi.Status,
            keterangan  : null);

        return MapToDetail(disposisi, surat, pemberi, penerima,
            request.ParentDisposisiId);
    }

    // ─────────────────────────────────────────────────────────
    // GET LIST DISPOSISI
    // ─────────────────────────────────────────────────────────
    public async Task<IEnumerable<DisposisiListResponse>> GetDisposisiListAsync(
        int? pemberiId, int? penerimaId)
    {
        var query = _context.Disposisis
            .Include(d => d.Surat)
            .Include(d => d.Pemberi)
            .Include(d => d.Penerima)
            .AsQueryable();

        if (pemberiId.HasValue)
            query = query.Where(d => d.PemberiId == pemberiId.Value);

        if (penerimaId.HasValue)
            query = query.Where(d => d.PenerimaId == penerimaId.Value);

        return await query
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new DisposisiListResponse
            {
                Id               = d.Id,
                SuratId          = d.SuratId,
                NoSurat          = d.Surat != null ? d.Surat.NoSurat : null,
                PerihalSurat     = d.Surat != null ? d.Surat.Perihal : string.Empty,
                NamaPemberi      = d.Pemberi != null ? d.Pemberi.Nama : string.Empty,
                NamaPenerima     = d.Penerima != null ? d.Penerima.Nama : string.Empty,
                TanggalDisposisi = d.TanggalDisposisi,
                SifatDisposisi   = d.SifatDisposisi,
                Status           = d.Status,
                HasLampiranSurat = d.Surat != null && d.Surat.NamaFile != null,
                CreatedAt        = d.CreatedAt
            })
            .ToListAsync();
    }

    // ─────────────────────────────────────────────────────────
    // GET DETAIL DISPOSISI
    // ─────────────────────────────────────────────────────────
    public async Task<DisposisiDetailResponse> GetDisposisiByIdAsync(int id)
    {
        var d = await _context.Disposisis
            .Include(d => d.Surat)
            .Include(d => d.Pemberi).ThenInclude(u => u.Role)
            .Include(d => d.Penerima).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new KeyNotFoundException(
                $"Disposisi ID {id} tidak ditemukan.");

        var relasiParent = await _context.DisposisiRelations
            .FirstOrDefaultAsync(r => r.ChildId == id);

        return MapToDetail(d, d.Surat, d.Pemberi, d.Penerima, relasiParent?.ParentId);
    }

    // ─────────────────────────────────────────────────────────
    // GET TRACKING
    // ─────────────────────────────────────────────────────────
    public async Task<DisposisiTrackingResponse> GetTrackingBySuratIdAsync(int suratId)
    {
        var surat = await _context.Surats.FindAsync(suratId)
            ?? throw new KeyNotFoundException(
                $"Surat ID {suratId} tidak ditemukan.");

        var semuaDisposisi = await _context.Disposisis
            .Include(d => d.Pemberi).ThenInclude(u => u.Role)
            .Include(d => d.Penerima).ThenInclude(u => u.Role)
            .Where(d => d.SuratId == suratId)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync();

        // Kumpulkan ID semua disposisi
        var disposisiIds = semuaDisposisi.Select(d => d.Id).ToList();

        // Ambil ParentId dari relasi — hasilnya List<int>
        var parentIds = await _context.DisposisiRelations
            .Where(r => disposisiIds.Contains(r.ParentId))
            .Select(r => r.ParentId)
            .ToListAsync();

        // HashSet<int> untuk lookup O(1)
        var punyaChild = parentIds.ToHashSet();

        var riwayat = semuaDisposisi.Select((d, index) => new TrackingStep
        {
            StepOrder      = index + 1,
            DisposisiId    = d.Id,
            NamaPemberi    = d.Pemberi?.Nama ?? string.Empty,
            RolePemberi    = d.Pemberi?.Role?.NamaRole,
            NamaPenerima   = d.Penerima?.Nama ?? string.Empty,
            RolePenerima   = d.Penerima?.Role?.NamaRole,
            SifatDisposisi = d.SifatDisposisi,
            Instruksi      = d.Instruksi,
            Status         = d.Status,
            Waktu          = d.CreatedAt,
            IsPosisiTerkini = !punyaChild.Contains(d.Id)  // ← d adalah Disposisi, d.Id adalah int
        }).ToList();

        var posisiTerkini = riwayat.LastOrDefault(s => s.IsPosisiTerkini);

        return new DisposisiTrackingResponse
        {
            SuratId           = suratId,
            NoSurat           = surat.NoSurat,
            PerihalSurat      = surat.Perihal,
            NamaFile          = surat.NamaFile,
            PosisiTerkini     = posisiTerkini?.NamaPenerima ?? "-",
            RolePosisiTerkini = posisiTerkini?.RolePenerima,
            StatusTerkini     = posisiTerkini?.Status ?? "-",
            Riwayat           = riwayat
        };
    }

    // ─────────────────────────────────────────────────────────
    // HELPER — MapToDetail
    // ─────────────────────────────────────────────────────────
    private static DisposisiDetailResponse MapToDetail(
        Disposisi d,
        Surat? surat,
        User? pemberi,
        User? penerima,
        int? parentId)
    {
        return new DisposisiDetailResponse
        {
            Id               = d.Id,
            SuratId          = d.SuratId,
            NoSurat          = surat?.NoSurat,
            PerihalSurat     = surat?.Perihal ?? string.Empty,
            NamaFile         = surat?.NamaFile,
            HasLampiran      = surat?.NamaFile != null,
            PemberiId        = d.PemberiId,
            NamaPemberi      = pemberi?.Nama ?? string.Empty,
            RolePemberi      = pemberi?.Role?.NamaRole,
            PenerimaId       = d.PenerimaId,
            NamaPenerima     = penerima?.Nama ?? string.Empty,
            RolePenerima     = penerima?.Role?.NamaRole,
            TanggalDisposisi = d.TanggalDisposisi,
            SifatDisposisi   = d.SifatDisposisi,
            Instruksi        = d.Instruksi,
            Status           = d.Status,
            WaktuDiterima    = d.WaktuDiterima,
            CompletedAt      = d.CompletedAt,
            CreatedAt        = d.CreatedAt,
            ParentDisposisiId = parentId
        };
    }

    // ─────────────────────────────────────────────────────────
    // HELPER — AddLogAsync (private, dipanggil tiap ada perubahan status)
    // ─────────────────────────────────────────────────────────
    private async Task AddLogAsync(
        int disposisiId, int suratId, int? userId,
        string aksi, string? statusLama, string? statusBaru, string? keterangan)
    {
        _context.DisposisiLogs.Add(new DisposisiLog
        {
            DisposisiId = disposisiId,
            SuratId     = suratId,
            UserId      = userId,
            Aksi        = aksi,
            StatusLama  = statusLama,
            StatusBaru  = statusBaru,
            Keterangan  = keterangan,
            CreatedAt   = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }

    // ─────────────────────────────────────────────────────────
    // TERIMA DISPOSISI (Pending → Accepted)
    // ─────────────────────────────────────────────────────────
    public async Task<DisposisiDetailResponse> TerimaDisposisiAsync(
        int disposisiId, UpdateDisposisiStatusRequest request)
    {
        var disposisi = await _context.Disposisis
            .Include(d => d.Surat)
            .Include(d => d.Pemberi).ThenInclude(u => u.Role)
            .Include(d => d.Penerima).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(d => d.Id == disposisiId)
            ?? throw new KeyNotFoundException(
                $"Disposisi ID {disposisiId} tidak ditemukan.");

        if (disposisi.Status != "Pending")
            throw new InvalidOperationException(
                $"Disposisi sudah dalam status '{disposisi.Status}'. Hanya disposisi berstatus Pending yang bisa diterima.");

        // Pastikan yang menerima adalah penerima disposisi
        if (disposisi.PenerimaId != request.UserId)
            throw new UnauthorizedAccessException(
                "Hanya penerima disposisi yang dapat menerima disposisi ini.");

        var statusLama = disposisi.Status;
        disposisi.Status       = "Accepted";
        disposisi.WaktuDiterima = DateTime.Now;
        await _context.SaveChangesAsync();

        await AddLogAsync(
            disposisiId : disposisi.Id,
            suratId     : disposisi.SuratId,
            userId      : request.UserId,
            aksi        : "DITERIMA",
            statusLama  : statusLama,
            statusBaru  : disposisi.Status,
            keterangan  : request.Keterangan);

        _logger.LogInformation(
            "Disposisi ID {Id} diterima oleh UserID {UserId}.",
            disposisiId, request.UserId);

        var relasiParent = await _context.DisposisiRelations
            .FirstOrDefaultAsync(r => r.ChildId == disposisiId);

        return MapToDetail(disposisi, disposisi.Surat, disposisi.Pemberi,
            disposisi.Penerima, relasiParent?.ParentId);
    }

    // ─────────────────────────────────────────────────────────
    // SELESAIKAN DISPOSISI (Accepted → Completed)
    // ─────────────────────────────────────────────────────────
    public async Task<DisposisiDetailResponse> SelesaikanDisposisiAsync(
        int disposisiId, UpdateDisposisiStatusRequest request)
    {
        var disposisi = await _context.Disposisis
            .Include(d => d.Surat)
            .Include(d => d.Pemberi).ThenInclude(u => u.Role)
            .Include(d => d.Penerima).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(d => d.Id == disposisiId)
            ?? throw new KeyNotFoundException(
                $"Disposisi ID {disposisiId} tidak ditemukan.");

        if (disposisi.Status != "Accepted")
            throw new InvalidOperationException(
                $"Disposisi dalam status '{disposisi.Status}'. Hanya disposisi berstatus Accepted yang bisa diselesaikan.");

        if (disposisi.PenerimaId != request.UserId)
            throw new UnauthorizedAccessException(
                "Hanya penerima disposisi yang dapat menyelesaikan disposisi ini.");

        var statusLama = disposisi.Status;
        disposisi.Status      = "Completed";
        disposisi.CompletedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        await AddLogAsync(
            disposisiId : disposisi.Id,
            suratId     : disposisi.SuratId,
            userId      : request.UserId,
            aksi        : "DISELESAIKAN",
            statusLama  : statusLama,
            statusBaru  : disposisi.Status,
            keterangan  : request.Keterangan);

        _logger.LogInformation(
            "Disposisi ID {Id} diselesaikan oleh UserID {UserId}.",
            disposisiId, request.UserId);

        var relasiParent = await _context.DisposisiRelations
            .FirstOrDefaultAsync(r => r.ChildId == disposisiId);

        return MapToDetail(disposisi, disposisi.Surat, disposisi.Pemberi,
            disposisi.Penerima, relasiParent?.ParentId);
    }

    // ─────────────────────────────────────────────────────────
    // GET LOG BY DISPOSISI ID
    // ─────────────────────────────────────────────────────────
    public async Task<IEnumerable<DisposisiLogResponse>> GetLogByDisposisiIdAsync(int disposisiId)
    {
        var exists = await _context.Disposisis.AnyAsync(d => d.Id == disposisiId);
        if (!exists)
            throw new KeyNotFoundException($"Disposisi ID {disposisiId} tidak ditemukan.");

        return await _context.DisposisiLogs
            .Include(l => l.User)
            .Where(l => l.DisposisiId == disposisiId)
            .OrderBy(l => l.CreatedAt)
            .Select(l => new DisposisiLogResponse
            {
                Id           = l.Id,
                DisposisiId  = l.DisposisiId,
                SuratId      = l.SuratId,
                NamaUser     = l.User != null ? l.User.Nama : null,
                Aksi         = l.Aksi,
                StatusLama   = l.StatusLama,
                StatusBaru   = l.StatusBaru,
                Keterangan   = l.Keterangan,
                CreatedAt    = l.CreatedAt
            })
            .ToListAsync();
    }

    // ─────────────────────────────────────────────────────────
    // GET LOG BY SURAT ID
    // ─────────────────────────────────────────────────────────
    public async Task<IEnumerable<DisposisiLogResponse>> GetLogBySuratIdAsync(int suratId)
    {
        var exists = await _context.Surats.AnyAsync(s => s.Id == suratId);
        if (!exists)
            throw new KeyNotFoundException($"Surat ID {suratId} tidak ditemukan.");

        return await _context.DisposisiLogs
            .Include(l => l.User)
            .Where(l => l.SuratId == suratId)
            .OrderBy(l => l.CreatedAt)
            .Select(l => new DisposisiLogResponse
            {
                Id           = l.Id,
                DisposisiId  = l.DisposisiId,
                SuratId      = l.SuratId,
                NamaUser     = l.User != null ? l.User.Nama : null,
                Aksi         = l.Aksi,
                StatusLama   = l.StatusLama,
                StatusBaru   = l.StatusBaru,
                Keterangan   = l.Keterangan,
                CreatedAt    = l.CreatedAt
            })
            .ToListAsync();
    }
}