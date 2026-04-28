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
        NomorAgenda = request.NomorAgenda,
        NomorSurat = request.NomorSurat,
        TanggalSurat = request.TanggalSurat,
        TanggalDiterima = request.TanggalDiterima,
        NamaPengirim = request.NamaPengirim,
        InstansiPengirim = request.InstansiPengirim,
        Perihal = request.Perihal,
        Deskripsi = request.Deskripsi,
        KodeKlasifikasi = request.KodeKlasifikasi,
        JenisSumber = request.JenisSumber,
        TingkatPrioritas = request.TingkatPrioritas,
        Status = request.Status,
        DibuatPada = DateTime.UtcNow
    };

    _db.SuratMasuk.Add(entity);
    await _db.SaveChangesAsync();

    return new SuratMasukItemDto
    {
        Id = entity.Id,
        NomorAgenda = entity.NomorAgenda,
        NomorSurat = entity.NomorSurat,
        TanggalSurat = entity.TanggalSurat,
        TanggalDiterima = entity.TanggalDiterima,
        NamaPengirim = entity.NamaPengirim,
        InstansiPengirim = entity.InstansiPengirim,
        Perihal = entity.Perihal,
        JenisSumber = entity.JenisSumber,
        Status = entity.Status
    };
}
    public async Task<PagedResponseDto<SuratMasukItemDto>> GetAllAsync(SuratMasukQueryDto request)
    {
        ValidateSort(request);

        IQueryable<SuratMasuk> query = _db.SuratMasuk.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.NomorAgenda))
            query = query.Where(x => x.NomorAgenda.Contains(request.NomorAgenda));

        if (!string.IsNullOrWhiteSpace(request.NomorSurat))
            query = query.Where(x => x.NomorSurat != null && x.NomorSurat.Contains(request.NomorSurat));

        if (!string.IsNullOrWhiteSpace(request.NamaPengirim))
            query = query.Where(x => x.NamaPengirim.Contains(request.NamaPengirim));

        if (!string.IsNullOrWhiteSpace(request.Perihal))
            query = query.Where(x => x.Perihal.Contains(request.Perihal));

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (!string.IsNullOrWhiteSpace(request.JenisSumber))
            query = query.Where(x => x.JenisSumber == request.JenisSumber);

        if (request.TanggalDiterimaDari.HasValue)
            query = query.Where(x => x.TanggalDiterima >= request.TanggalDiterimaDari.Value);

        if (request.TanggalDiterimaSampai.HasValue)
            query = query.Where(x => x.TanggalDiterima <= request.TanggalDiterimaSampai.Value);

        query = ApplySorting(query, request.SortBy, request.SortDirection);

        var totalData = await query.CountAsync();

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SuratMasukItemDto
            {
                Id = x.Id,
                NomorAgenda = x.NomorAgenda,
                NomorSurat = x.NomorSurat,
                TanggalSurat = x.TanggalSurat,
                TanggalDiterima = x.TanggalDiterima,
                NamaPengirim = x.NamaPengirim,
                InstansiPengirim = x.InstansiPengirim,
                Perihal = x.Perihal,
                JenisSumber = x.JenisSumber,
                Status = x.Status
            })
            .ToListAsync();

        return new PagedResponseDto<SuratMasukItemDto>
        {
            Data = items,
            Meta = new PaginationMeta
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalData = totalData,
                TotalPage = (int)Math.Ceiling((double)totalData / request.PageSize),
                SortBy = request.SortBy,
                SortDirection = request.SortDirection
            }
        };
    }

   private static IQueryable<SuratMasuk> ApplySorting(IQueryable<SuratMasuk> query, string sortBy, string sortDirection)
{
    var isAsc = sortDirection.ToLower() == "asc";

    return sortBy.ToLower() switch
    {
        "nomor_agenda" => isAsc ? query.OrderBy(x => x.NomorAgenda) : query.OrderByDescending(x => x.NomorAgenda),
        "nomor_surat" => isAsc ? query.OrderBy(x => x.NomorSurat) : query.OrderByDescending(x => x.NomorSurat),
        "tanggal_surat" => isAsc ? query.OrderBy(x => x.TanggalSurat) : query.OrderByDescending(x => x.TanggalSurat),
        "tanggal_diterima" => isAsc ? query.OrderBy(x => x.TanggalDiterima) : query.OrderByDescending(x => x.TanggalDiterima),
        "nama_pengirim" => isAsc ? query.OrderBy(x => x.NamaPengirim) : query.OrderByDescending(x => x.NamaPengirim),
        "status" => isAsc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
        "jenis_sumber" => isAsc ? query.OrderBy(x => x.JenisSumber) : query.OrderByDescending(x => x.JenisSumber),
        _ => isAsc ? query.OrderBy(x => x.TanggalDiterima) : query.OrderByDescending(x => x.TanggalDiterima)
    };
}
    private static void ValidateSort(SuratMasukQueryDto request)
    {
        var allowedSortBy = new[]
        {
            "tanggal_diterima",
            "nomor_agenda",
            "nomor_surat",
            "tanggal_surat",
            "nama_pengirim",
            "status",
            "jenis_sumber"
        };

        if (!allowedSortBy.Contains(request.SortBy.ToLower()))
            throw new ArgumentException("sortBy tidak valid.");

        if (request.SortDirection.ToLower() is not ("asc" or "desc"))
            throw new ArgumentException("sortDirection harus asc atau desc.");

        if (request.TanggalDiterimaDari.HasValue &&
            request.TanggalDiterimaSampai.HasValue &&
            request.TanggalDiterimaDari > request.TanggalDiterimaSampai)
            throw new ArgumentException("TanggalDiterimaDari tidak boleh lebih besar dari TanggalDiterimaSampai.");
    }
}