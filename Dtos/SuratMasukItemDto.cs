namespace Maildesk.Api.Dtos;

public class SuratMasukItemDto
{
    public long Id { get; set; }
    public string NomorAgenda { get; set; } = "";
    public string? NomorSurat { get; set; }
    public DateOnly? TanggalSurat { get; set; }
    public DateOnly TanggalDiterima { get; set; }
    public string NamaPengirim { get; set; } = "";
    public string? InstansiPengirim { get; set; }
    public string Perihal { get; set; } = "";
    public string? JenisSumber { get; set; }
    public string Status { get; set; } = "";
}