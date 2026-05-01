namespace MailDesk.API.DTOs.SuratMasuk;

public class SuratMasukResponse
{
    public int Id { get; set; }
    public string NoSurat { get; set; } = string.Empty;
    public string NomorAgenda { get; set; } = string.Empty;
    public DateOnly TanggalSurat { get; set; }
    public string AsalPengirim { get; set; } = string.Empty;
    public string Perihal { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public string? PencatatNama { get; set; }  // nama user yang mencatat
    public DateTime CreatedAt { get; set; }
}