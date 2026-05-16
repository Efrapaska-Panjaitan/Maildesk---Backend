using System.ComponentModel.DataAnnotations;

namespace MailDesk.API.DTOs.Inbox;

public class CreateInboxFromSuratRequest
{
    [Required]
    public int SuratId { get; set; }

    /// <summary>
    /// Daftar ID penerima inbox. Minimal 1, maksimal 5 user.
    /// </summary>
    [Required(ErrorMessage = "PenerimaIds wajib diisi.")]
    [MinLength(1, ErrorMessage = "Minimal 1 penerima harus ditentukan.")]
    [MaxLength(5, ErrorMessage = "Maksimal hanya 5 penerima yang diizinkan untuk mencegah spam.")]
    public List<int> PenerimaIds { get; set; } = new();

    public string? CatatanPengantar { get; set; }
}
