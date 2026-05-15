namespace MailDesk.API.DTOs.Inbox;

public class InboxDetailResponse
{
    public int Id { get; set; }
    public int? SuratId { get; set; }
    public int? PengirimId { get; set; }
    public int? PenerimaId { get; set; }

    public string? NomorAgenda { get; set; }
    public string? NoSurat { get; set; }
    public string? Pengirim { get; set; }
    public string? PenerimaName { get; set; }
    public string? Perihal { get; set; }
    public string? KategoriSurat { get; set; }
    public DateOnly? TanggalSurat { get; set; }

    public string Status { get; set; } = string.Empty;
    public string? CatatanPengantar { get; set; }
    public string? NamaFile { get; set; }
    public string? FileUrl { get; set; }

    public DateTime CreatedAt { get; set; }
}
