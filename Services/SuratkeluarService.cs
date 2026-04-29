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

    public async Task<PagedResponseDto<SuratKeluarItemDto>> GetAllAsync(SuratKeluarQueryDto request)
    {
        ValidateSort(request);

        IQueryable<SuratKeluar> query = _db.SuratKeluar.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.NomorAgenda))
            query = query.Where(x => x.NomorAgenda.Contains(request.NomorAgenda));

        if (!string.IsNullOrWhiteSpace(request.NomorSurat))
            query = query.Where(x => x.NomorSurat.Contains(request.NomorSurat));

        if (!string.IsNullOrWhiteSpace(request.TujuanSurat))
            query = query.Where(x => x.TujuanSurat.Contains(request.TujuanSurat));

        if (!string.IsNullOrWhiteSpace(request.InstansiTujuan))
            query = query.Where(x => x.InstansiTujuan != null && x.InstansiTujuan.Contains(request.InstansiTujuan));

        if (!string.IsNullOrWhiteSpace(request.Perihal))
            query = query.Where(x => x.Perihal.Contains(request.Perihal));

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (request.TanggalSuratDari.HasValue)
            query = query.Where(x => x.TanggalSurat >= request.TanggalSuratDari.Value);

        if (request.TanggalSuratSampai.HasValue)
            query = query.Where(x => x.TanggalSurat <= request.TanggalSuratSampai.Value);

        query = ApplySorting(query, request.SortBy, request.SortDirection);

        var totalData = await query.CountAsync();

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SuratKeluarItemDto
            {
                Id = x.Id,
                NomorAgenda = x.NomorAgenda,
                NomorSurat = x.NomorSurat,
                TanggalSurat = x.TanggalSurat,
                TujuanSurat = x.TujuanSurat,
                InstansiTujuan = x.InstansiTujuan,
                Perihal = x.Perihal,
                IsiRingkas = x.IsiRingkas,
                KodeKlasifikasi = x.KodeKlasifikasi,
                TingkatPrioritas = x.TingkatPrioritas,
                Status = x.Status,
                DibuatPada = x.DibuatPada,
                DiperbaruiPada = x.DiperbaruiPada
            })
            .ToListAsync();

        return new PagedResponseDto<SuratKeluarItemDto>
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

    private static IQueryable<SuratKeluar> ApplySorting(IQueryable<SuratKeluar> query, string sortBy, string sortDirection)
    {
        var isAsc = sortDirection.ToLower() == "asc";

        return sortBy.ToLower() switch
        {
            "nomor_agenda" => isAsc ? query.OrderBy(x => x.NomorAgenda) : query.OrderByDescending(x => x.NomorAgenda),
            "nomor_surat" => isAsc ? query.OrderBy(x => x.NomorSurat) : query.OrderByDescending(x => x.NomorSurat),
            "tanggal_surat" => isAsc ? query.OrderBy(x => x.TanggalSurat) : query.OrderByDescending(x => x.TanggalSurat),
            "tujuan_surat" => isAsc ? query.OrderBy(x => x.TujuanSurat) : query.OrderByDescending(x => x.TujuanSurat),
            "instansi_tujuan" => isAsc ? query.OrderBy(x => x.InstansiTujuan) : query.OrderByDescending(x => x.InstansiTujuan),
            "perihal" => isAsc ? query.OrderBy(x => x.Perihal) : query.OrderByDescending(x => x.Perihal),
            "status" => isAsc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
            _ => isAsc ? query.OrderBy(x => x.TanggalSurat) : query.OrderByDescending(x => x.TanggalSurat)
        };
    }

    private static void ValidateSort(SuratKeluarQueryDto request)
    {
        var allowedSortBy = new[]
        {
            "tanggal_surat",
            "nomor_agenda",
            "nomor_surat",
            "tujuan_surat",
            "instansi_tujuan",
            "perihal",
            "status"
        };

        if (!allowedSortBy.Contains(request.SortBy.ToLower()))
            throw new ArgumentException("sortBy tidak valid.");

        if (request.SortDirection.ToLower() is not ("asc" or "desc"))
            throw new ArgumentException("sortDirection harus asc atau desc.");

        if (request.TanggalSuratDari.HasValue &&
            request.TanggalSuratSampai.HasValue &&
            request.TanggalSuratDari > request.TanggalSuratSampai)
            throw new ArgumentException("TanggalSuratDari tidak boleh lebih besar dari TanggalSuratSampai.");
    }
}