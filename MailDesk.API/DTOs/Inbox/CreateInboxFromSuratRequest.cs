namespace MailDesk.API.DTOs.Inbox;

public class CreateInboxFromSuratRequest
{
    public int SuratId { get; set; }
    public int? PenerimaId { get; set; }
    public string? CatatanPengantar { get; set; }
}
