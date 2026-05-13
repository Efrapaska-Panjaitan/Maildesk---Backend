using System.ComponentModel.DataAnnotations;

namespace Maildesk.Api.Dtos;

public class SuratKeluarQueryDto
{
    public string? NoSurat { get; set; }
    public string? NomorAgenda { get; set; }
    public string? Kepada { get; set; }
    public string? Perihal { get; set; }
    public string? Status { get; set; }
    public bool? IsArchived { get; set; }

    public DateOnly? TanggalSuratDari { get; set; }
    public DateOnly? TanggalSuratSampai { get; set; }

    public string SortBy { get; set; } = "tanggal_surat";
    public string SortDirection { get; set; } = "desc";

    [Range(1, 1000)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}