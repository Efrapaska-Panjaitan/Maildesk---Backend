using System.ComponentModel.DataAnnotations;

namespace MailDesk.API.DTOs.Surat;

/// <summary>
/// Request untuk mencatat surat masuk baru.
/// Dikirim sebagai multipart/form-data — semua field surat + file PDF dalam satu request.
/// </summary>
public class CreateSuratWithFileRequest : CreateSuratRequest
{
    /// <summary>
    /// File PDF lampiran surat (wajib, maksimal 10MB).
    /// </summary>
    [Required(ErrorMessage = "File PDF wajib dilampirkan.")]
    public IFormFile File { get; set; } = null!;
}
