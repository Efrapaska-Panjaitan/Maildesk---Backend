namespace MailDesk.API.DTOs.Inbox;

public class InboxQueryParams
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
    public string SortBy { get; set; } = "tanggal";
    public string SortOrder { get; set; } = "desc";
    public string? Status { get; set; }
    public string? Search { get; set; }
    public DateOnly? TanggalDari { get; set; }
    public DateOnly? TanggalSampai { get; set; }

    // Filter khusus: jika diisi, hanya tampilkan inbox dengan PenerimaId = FilterByPenerimaId
    // Digunakan untuk Pimpinan/User agar hanya lihat inbox mereka
    public int? FilterByPenerimaId { get; set; }
}
