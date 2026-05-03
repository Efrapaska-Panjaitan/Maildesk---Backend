namespace Maildesk.Api.Dtos;

public class SuratKeluarItemDto
{
    public int Id { get; set; }
    public string? NoSurat { get; set; }
    public string? NomorAgenda { get; set; }
    public DateOnly? TanggalSurat { get; set; }
    public string Kepada { get; set; } = string.Empty;
    public string Perihal { get; set; } = string.Empty;
    public string? NamaFile { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
}