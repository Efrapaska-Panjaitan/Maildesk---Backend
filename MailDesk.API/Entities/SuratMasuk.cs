namespace Maildesk.Api.Entities;

public class SuratMasuk
{
    public int Id { get; set; }
    public string NoSurat { get; set; } = string.Empty;
    public string? NomorAgenda { get; set; }
    public DateOnly TanggalSurat { get; set; }
    public string AsalPengirim { get; set; } = string.Empty;
    public string Perihal { get; set; } = string.Empty;
    public byte[]? FileLampiran { get; set; }
    public string? NamaFile { get; set; }
    public bool IsArchived { get; set; } = false;
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}