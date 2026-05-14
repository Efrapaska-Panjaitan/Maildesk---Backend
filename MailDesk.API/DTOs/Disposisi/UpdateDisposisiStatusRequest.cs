using System.ComponentModel.DataAnnotations;

namespace MailDesk.API.DTOs.Disposisi;

/// <summary>
/// Request body untuk endpoint update status disposisi
/// (PATCH /api/disposisi/{id}/terima dan /selesai).
/// </summary>
public class UpdateDisposisiStatusRequest
{
    /// <summary>
    /// ID user yang melakukan aksi (penerima disposisi yang klik Terima/Selesai).
    /// </summary>
    [Required(ErrorMessage = "UserId wajib diisi.")]
    public int UserId { get; set; }

    /// <summary>Catatan tambahan opsional dari user saat melakukan aksi.</summary>
    [MaxLength(500)]
    public string? Keterangan { get; set; }
}
