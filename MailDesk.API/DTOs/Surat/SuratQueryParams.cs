namespace MailDesk.API.DTOs.Surat;

public class SuratQueryParams
{
    // ── Pagination ────────────────────────────────────────────
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 50;

    // ── Sorting ───────────────────────────────────────────────
    // Nilai: tanggal | nomor_agenda | pengirim | status
    public string SortBy { get; set; } = "tanggal";

    // Nilai: asc | desc
    public string SortOrder { get; set; } = "desc";

    // ── Filter ────────────────────────────────────────────────
    public string? Status { get; set; }           // Baru, Diproses, Selesai
    public string? KategoriSurat { get; set; }    // Undangan, Edaran, dll
    public string? Search { get; set; }           // Search perihal/pengirim/no_surat
    public DateOnly? TanggalDari { get; set; }    // Filter tanggal mulai
    public DateOnly? TanggalSampai { get; set; }  // Filter tanggal akhir
    public bool IncludeArchived { get; set; } = false; // Default tidak tampilkan arsip
}