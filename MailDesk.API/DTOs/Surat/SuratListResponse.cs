namespace MailDesk.API.DTOs.Surat;

public class SuratListResponse
{
    public int Id { get; set; }
    public string? NoSurat { get; set; }
    public string NomorAgenda { get; set; } = string.Empty;
    public string JenisSurat { get; set; } = string.Empty;
    public string? KategoriSurat { get; set; }
    public DateOnly TanggalSurat { get; set; }
    public string Pengirim { get; set; } = string.Empty;
    public string Penerima { get; set; } = string.Empty;
    public string Perihal { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool HasLampiran { get; set; }

    public DateTime CreatedAt { get; set; }
}