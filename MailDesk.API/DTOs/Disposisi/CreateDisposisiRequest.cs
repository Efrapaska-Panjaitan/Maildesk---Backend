using System.ComponentModel.DataAnnotations;

namespace MailDesk.API.DTOs.Disposisi;

public class CreateDisposisiRequest
{
    [Required(ErrorMessage = "ID surat wajib diisi.")]
    public int SuratId { get; set; }

    [Required(ErrorMessage = "ID pemberi disposisi wajib diisi.")]
    public int PemberiId { get; set; }

    [Required(ErrorMessage = "ID penerima disposisi wajib diisi.")]
    public int PenerimaId { get; set; }

    [Required(ErrorMessage = "Tanggal disposisi wajib diisi.")]
    public DateOnly TanggalDisposisi { get; set; }

    [Required(ErrorMessage = "Sifat disposisi wajib diisi.")]
    public string SifatDisposisi { get; set; } = "Biasa"; // Biasa | Penting | Mendesak | Rahasia

    public string? Instruksi { get; set; }

    // ID disposisi sebelumnya (opsional, untuk chain tracking)
    public int? ParentDisposisiId { get; set; }
}