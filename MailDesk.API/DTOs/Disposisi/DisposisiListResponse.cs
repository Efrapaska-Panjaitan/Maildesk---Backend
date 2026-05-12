namespace MailDesk.API.DTOs.Disposisi;

public class DisposisiListResponse
{
    public int Id { get; set; }
    public int SuratId { get; set; }
    public string? NoSurat { get; set; }
    public string PerihalSurat { get; set; } = string.Empty;
    public string NamaPemberi { get; set; } = string.Empty;
    public string NamaPenerima { get; set; } = string.Empty;
    public DateOnly TanggalDisposisi { get; set; }
    public string SifatDisposisi { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool HasLampiranSurat { get; set; }
    public DateTime CreatedAt { get; set; }
}