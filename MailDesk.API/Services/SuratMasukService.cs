using Maildesk.Api.Data;
using Maildesk.Api.Dtos;
using Maildesk.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maildesk.Api.Services;

public class SuratMasukService
{
    private readonly MaildeskDbContext _db;

    public SuratMasukService(MaildeskDbContext db)
    {
        _db = db;
    }

    public async Task<SuratMasukItemDto> CreateAsync(CreateSuratMasukDto request)
    {
        var entity = new SuratMasuk
        {
            NoSurat       = request.NoSurat,
            NomorAgenda   = request.NomorAgenda,
            TanggalSurat  = request.TanggalSurat,
            AsalPengirim  = request.AsalPengirim,
            Perihal       = request.Perihal,
            NamaFile      = request.NamaFile,
            IsArchived    = false,
            CreatedAt     = DateTime.UtcNow
        };

        _db.SuratMasuk.Add(entity);
        await _db.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<PagedResponseDto<SuratMasukItemDto>> GetAllAsync(SuratMasukQueryDto request)
    {
        ValidateSort(request);

        IQueryable<SuratMasuk> query = _db.SuratMasuk.AsNoTracking();

        // --- Filter ---
        if (!string.IsNullOrWhiteSpace(request.NoSurat))
            query = query.Where(x => x.NoSurat.Contains(request.NoSurat));

        if (!string.IsNullOrWhiteSpace(request.NomorAgenda))
            query = query.Where(x => x.NomorAgenda != null && x.NomorAgenda.Contains(request.NomorAgenda));

        if (!string.IsNullOrWhiteSpace(request.AsalPengirim))
            query = query.Where(x => x.AsalPengirim.Contains(request.AsalPengirim));

        if (!string.IsNullOrWhiteSpace(request.Perihal))
            query = query.Where(x => x.Perihal.Contains(request.Perihal));

        if (request.IsArchived.HasValue)
            query = query.Where(x => x.IsArchived == request.IsArchived.Value);

        if (request.TanggalSuratDari.HasValue)
            query = query.Where(x => x.TanggalSurat >= request.TanggalSuratDari.Value);

        if (request.TanggalSuratSampai.HasValue)
            query = query.Where(x => x.TanggalSurat <= request.TanggalSuratSampai.Value);

        // --- Sorting ---
        query = ApplySorting(query, request.SortBy, request.SortDirection);

        // --- Pagination ---
        var totalData = await query.CountAsync();

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => MapToDto(x))
            .ToListAsync();

        return new PagedResponseDto<SuratMasukItemDto>
        {
            Data = items,
            Meta = new PaginationMeta
            {
                Page          = request.Page,
                PageSize      = request.PageSize,
                TotalData     = totalData,
                TotalPage     = (int)Math.Ceiling((double)totalData / request.PageSize),
                SortBy        = request.SortBy,
                SortDirection = request.SortDirection
            }
        };
    }

    private static SuratMasukItemDto MapToDto(SuratMasuk x) => new()
    {
        Id           = x.Id,
        NoSurat      = x.NoSurat,
        NomorAgenda  = x.NomorAgenda,
        TanggalSurat = x.TanggalSurat,
        AsalPengirim = x.AsalPengirim,
        Perihal      = x.Perihal,
        NamaFile     = x.NamaFile,
        IsArchived   = x.IsArchived,
        CreatedAt    = x.CreatedAt
    };

    private static IQueryable<SuratMasuk> ApplySorting(
        IQueryable<SuratMasuk> query, string sortBy, string sortDirection)
    {
        var asc = sortDirection.ToLower() == "asc";
        return sortBy.ToLower() switch
        {
            "no_surat"      => asc ? query.OrderBy(x => x.NoSurat)      : query.OrderByDescending(x => x.NoSurat),
            "nomor_agenda"  => asc ? query.OrderBy(x => x.NomorAgenda)  : query.OrderByDescending(x => x.NomorAgenda),
            "asal_pengirim" => asc ? query.OrderBy(x => x.AsalPengirim) : query.OrderByDescending(x => x.AsalPengirim),
            "perihal"       => asc ? query.OrderBy(x => x.Perihal)      : query.OrderByDescending(x => x.Perihal),
            "is_archived"   => asc ? query.OrderBy(x => x.IsArchived)   : query.OrderByDescending(x => x.IsArchived),
            "created_at"    => asc ? query.OrderBy(x => x.CreatedAt)    : query.OrderByDescending(x => x.CreatedAt),
            _               => asc ? query.OrderBy(x => x.TanggalSurat) : query.OrderByDescending(x => x.TanggalSurat)
        };
    }

    private static void ValidateSort(SuratMasukQueryDto request)
    {
        var allowed = new[]
        {
            "tanggal_surat", "no_surat", "nomor_agenda",
            "asal_pengirim", "perihal", "is_archived", "created_at"
        };

        if (!allowed.Contains(request.SortBy.ToLower()))
            throw new ArgumentException("sortBy tidak valid.");

        if (request.SortDirection.ToLower() is not ("asc" or "desc"))
            throw new ArgumentException("sortDirection harus asc atau desc.");

        if (request.TanggalSuratDari.HasValue &&
            request.TanggalSuratSampai.HasValue &&
            request.TanggalSuratDari > request.TanggalSuratSampai)
            throw new ArgumentException("TanggalSuratDari tidak boleh lebih besar dari TanggalSuratSampai.");
    }
}