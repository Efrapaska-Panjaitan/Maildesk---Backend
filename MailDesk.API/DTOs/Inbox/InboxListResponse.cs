namespace MailDesk.API.DTOs.Inbox;

public class InboxListResponse
{
    public int Id { get; set; }
    public int? SuratId { get; set; }
    public string? NomorAgenda { get; set; }
    /// <summary>Nomor surat resmi dari kolom surat.NoSurat.</summary>
    public string? NoSurat { get; set; }
    public string? Pengirim { get; set; }
    public string? Perihal { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CatatanPengantar { get; set; }
    /// <summary>Nama file PDF lampiran surat.</summary>
    public string? NamaFile { get; set; }
    public DateTime CreatedAt { get; set; }
}
