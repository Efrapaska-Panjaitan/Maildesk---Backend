using System.ComponentModel.DataAnnotations;

namespace Maildesk.Api.Dtos;

public class SuratKeluarQueryDto
{
    [StringLength(100)]
    public string? NomorAgenda { get; set; }

    [StringLength(150)]
    public string? NomorSurat { get; set; }

    [StringLength(200)]
    public string? TujuanSurat { get; set; }

    [StringLength(200)]
    public string? InstansiTujuan { get; set; }

    [StringLength(255)]
    public string? Perihal { get; set; }

    [StringLength(30)]
    public string? Status { get; set; }

    public DateOnly? TanggalSuratDari { get; set; }

    public DateOnly? TanggalSuratSampai { get; set; }

    public string SortBy { get; set; } = "tanggal_surat";

    public string SortDirection { get; set; } = "desc";

    [Range(1, 1000)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}