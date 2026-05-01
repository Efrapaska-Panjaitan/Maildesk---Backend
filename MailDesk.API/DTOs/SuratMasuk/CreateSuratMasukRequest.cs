using System.ComponentModel.DataAnnotations;

namespace MailDesk.API.DTOs.SuratMasuk;

public class CreateSuratMasukRequest
{
    /// <summary>
    /// Nomor surat dari pengirim. Contoh: 421.3/B.1/DISDIK/2026
    /// </summary>
    [Required(ErrorMessage = "Nomor surat wajib diisi.")]
    [MaxLength(100, ErrorMessage = "Nomor surat maksimal 100 karakter.")]
    public string NoSurat { get; set; } = string.Empty;

    /// <summary>
    /// Tanggal tercantum pada surat. Format: yyyy-MM-dd
    /// </summary>
    [Required(ErrorMessage = "Tanggal surat wajib diisi.")]
    public DateOnly TanggalSurat { get; set; }

    /// <summary>
    /// Nama instansi / orang pengirim surat.
    /// </summary>
    [Required(ErrorMessage = "Asal pengirim wajib diisi.")]
    [MaxLength(150, ErrorMessage = "Asal pengirim maksimal 150 karakter.")]
    public string AsalPengirim { get; set; } = string.Empty;

    /// <summary>
    /// Perihal / subject surat.
    /// </summary>
    [Required(ErrorMessage = "Perihal wajib diisi.")]
    public string Perihal { get; set; } = string.Empty;

    /// <summary>
    /// ID user yang mencatat surat (biasanya TU/Sekretaris yang login).
    /// </summary>
    [Required(ErrorMessage = "User ID pencatat wajib diisi.")]
    public int UserId { get; set; }

    // NomorAgenda tidak diisi manual — digenerate otomatis oleh sistem
}