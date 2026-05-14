namespace Maildesk.Api.Entities;

public class SuratKeluar
{
    public int Id { get; set; }
    public string? NoSurat { get; set; }
    public string? NomorAgenda { get; set; }
    public DateOnly? TanggalSurat { get; set; }
    public string Kepada { get; set; } = string.Empty;
    public string Perihal { get; set; } = string.Empty;
    public byte[]? FileLampiran { get; set; }
    public string? NamaFile { get; set; }
    public string Status { get; set; } = "Draft";
    public bool IsArchived { get; set; } = false;
    public int? PembuatId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}