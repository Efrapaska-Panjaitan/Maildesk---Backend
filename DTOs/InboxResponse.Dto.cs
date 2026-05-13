namespace MailDesk.Api.DTOs;

public class InboxResponseDto
{
    public int IdInbox { get; set; }
    public int IdSurat { get; set; }
    public string NoSurat { get; set; } = string.Empty;
    public string Perihal { get; set; } = string.Empty;
    public string NamaPengirim { get; set; } = string.Empty;
    public DateTime? WaktuMasuk { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CatatanPengantar { get; set; } = string.Empty;
}