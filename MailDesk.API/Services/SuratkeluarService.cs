using Maildesk.Api.Data;
using Maildesk.Api.Dtos;
using Maildesk.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maildesk.Api.Services;

public class SuratKeluarService
{
    private readonly MaildeskDbContext _db;

    public SuratKeluarService(MaildeskDbContext db)
    {
        _db = db;
    }

    public async Task<SuratKeluarItemDto> CreateAsync(CreateSuratKeluarDto request)
    {
        var entity = new SuratKeluar
        {
            NoSurat      = request.NoSurat,
            NomorAgenda  = request.NomorAgenda,
            TanggalSurat = request.TanggalSurat,
            Kepada       = request.Kepada,
            Perihal      = request.Perihal,
            NamaFile     = request.NamaFile,
            Status       = request.Status,
            IsArchived   = false,
            CreatedAt    = DateTime.UtcNow
        };

        _db.SuratKeluar.Add(entity);
        await _db.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<PagedResponseDto<SuratKeluarItemDto>> GetAllAsync(SuratKeluarQueryDto request)
    {
        ValidateSort(request);

        IQueryable<SuratKeluar> query = _db.SuratKeluar.AsNoTracking();

        // --- Filter ---
        if (!string.IsNullOrWhiteSpace(request.NoSurat))
            query = query.Where(x => x.NoSurat != null && x.NoSurat.Contains(request.NoSurat));

        if (!string.IsNullOrWhiteSpace(request.NomorAgenda))
            query = query.Where(x => x.NomorAgenda != null && x.NomorAgenda.Contains(request.NomorAgenda));

        if (!string.IsNullOrWhiteSpace(request.Kepada))
            query = query.Where(x => x.Kepada.Contains(request.Kepada));

        if (!string.IsNullOrWhiteSpace(request.Perihal))
            query = query.Where(x => x.Perihal.Contains(request.Perihal));

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(x => x.Status == request.Status);

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

        return new PagedResponseDto<SuratKeluarItemDto>
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

    private static SuratKeluarItemDto MapToDto(SuratKeluar x) => new()
    {
        Id           = x.Id,
        NoSurat      = x.NoSurat,
        NomorAgenda  = x.NomorAgenda,
        TanggalSurat = x.TanggalSurat,
        Kepada       = x.Kepada,
        Perihal      = x.Perihal,
        NamaFile     = x.NamaFile,
        Status       = x.Status,
        IsArchived   = x.IsArchived,
        CreatedAt    = x.CreatedAt
    };

    private static IQueryable<SuratKeluar> ApplySorting(
        IQueryable<SuratKeluar> query, string sortBy, string sortDirection)
    {
        var asc = sortDirection.ToLower() == "asc";
        return sortBy.ToLower() switch
        {
            "no_surat"     => asc ? query.OrderBy(x => x.NoSurat)      : query.OrderByDescending(x => x.NoSurat),
            "nomor_agenda" => asc ? query.OrderBy(x => x.NomorAgenda)  : query.OrderByDescending(x => x.NomorAgenda),
            "kepada"       => asc ? query.OrderBy(x => x.Kepada)       : query.OrderByDescending(x => x.Kepada),
            "perihal"      => asc ? query.OrderBy(x => x.Perihal)      : query.OrderByDescending(x => x.Perihal),
            "status"       => asc ? query.OrderBy(x => x.Status)       : query.OrderByDescending(x => x.Status),
            "is_archived"  => asc ? query.OrderBy(x => x.IsArchived)   : query.OrderByDescending(x => x.IsArchived),
            "created_at"   => asc ? query.OrderBy(x => x.CreatedAt)    : query.OrderByDescending(x => x.CreatedAt),
            _              => asc ? query.OrderBy(x => x.TanggalSurat) : query.OrderByDescending(x => x.TanggalSurat)
        };
    }

    private static void ValidateSort(SuratKeluarQueryDto request)
    {
        var allowed = new[]
        {
            "tanggal_surat", "no_surat", "nomor_agenda",
            "kepada", "perihal", "status", "is_archived", "created_at"
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