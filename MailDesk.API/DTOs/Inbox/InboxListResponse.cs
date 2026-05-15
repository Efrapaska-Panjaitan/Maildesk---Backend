namespace MailDesk.API.DTOs.Inbox;

public class InboxListResponse
{
    public int Id { get; set; }
    public int? SuratId { get; set; }
    public string? NomorAgenda { get; set; }
    public string? Pengirim { get; set; }
    public string? Perihal { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CatatanPengantar { get; set; }
    public DateTime CreatedAt { get; set; }
}
