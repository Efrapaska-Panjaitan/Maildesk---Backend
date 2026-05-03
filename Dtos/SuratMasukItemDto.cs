namespace Maildesk.Api.Dtos;

public class SuratMasukItemDto
{
    public int Id { get; set; }
    public string NoSurat { get; set; } = string.Empty;
    public string? NomorAgenda { get; set; }
    public DateOnly TanggalSurat { get; set; }
    public string AsalPengirim { get; set; } = string.Empty;
    public string Perihal { get; set; } = string.Empty;
    public string? NamaFile { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
}