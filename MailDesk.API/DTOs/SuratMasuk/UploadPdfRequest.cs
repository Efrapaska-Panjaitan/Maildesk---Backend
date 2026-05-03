namespace MailDesk.API.DTOs.SuratMasuk;

public class UploadPdfResponse
{
    public int SuratMasukId { get; set; }
    public string NamaFile { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string FileSizeDisplay => FileSizeBytes < 1024 * 1024
        ? $"{FileSizeBytes / 1024} KB"
        : $"{FileSizeBytes / (1024 * 1024)} MB";
    public DateTime UploadedAt { get; set; }
}