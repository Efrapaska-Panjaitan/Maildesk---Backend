using System.ComponentModel.DataAnnotations;

namespace Maildesk.Api.Dtos;

public class SuratMasukQueryDto
{
    public string? NomorAgenda { get; set; }
    public string? NomorSurat { get; set; }
    public string? NamaPengirim { get; set; }
    public string? Perihal { get; set; }
    public string? Status { get; set; }
    public string? JenisSumber { get; set; }

    public DateOnly? TanggalDiterimaDari { get; set; }
    public DateOnly? TanggalDiterimaSampai { get; set; }

    public string SortBy { get; set; } = "tanggal_diterima";
    public string SortDirection { get; set; } = "desc";

    [Range(1, 1000)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}