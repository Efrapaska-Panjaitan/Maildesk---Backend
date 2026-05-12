namespace MailDesk.API.DTOs.Surat;

public class UploadPdfResponse
{
    public int SuratId { get; set; }

    /// <summary>Nama file asli dari pengguna (contoh: surat_undangan.pdf)</summary>
    public string NamaFileAsli { get; set; } = string.Empty;

    /// <summary>Path relatif untuk konstruksi URL (contoh: uploads/surat/2026/05/{uuid}.pdf)</summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>URL lengkap untuk mengakses file (contoh: http://localhost:5000/uploads/surat/2026/05/{uuid}.pdf)</summary>
    public string FileUrl { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }
    public string FileSizeDisplay => FileSizeBytes < 1024 * 1024
        ? $"{FileSizeBytes / 1024} KB"
        : $"{FileSizeBytes / (1024 * 1024)} MB";
    public DateTime UploadedAt { get; set; }
}