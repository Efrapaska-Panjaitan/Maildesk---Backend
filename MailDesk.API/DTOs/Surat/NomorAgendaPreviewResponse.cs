namespace MailDesk.API.DTOs.Surat;

public class NomorAgendaPreviewResponse
{
    /// <summary>
    /// Nomor agenda preview yang akan tampil di UI
    /// </summary>
    public string NomorAgenda { get; set; } = string.Empty;

    /// <summary>
    /// Informasi bahwa ini hanya preview, bukan nomor final
    /// </summary>
    public string Keterangan { get; set; } = "Preview - nomor final dikonfirmasi saat simpan";

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}