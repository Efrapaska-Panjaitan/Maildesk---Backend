using System.ComponentModel.DataAnnotations;

namespace MailDesk.API.DTOs.Surat;

public class CreateSuratRequest
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
    [Required(ErrorMessage = "Pengirim wajib diisi.")]
    [MaxLength(150, ErrorMessage = "Pengirim maksimal 150 karakter.")]
    public string Pengirim { get; set; } = string.Empty;

    [Required(ErrorMessage = "Penerima wajib diisi.")]
    [MaxLength(150)]
    public string Penerima { get; set; } = string.Empty;

    /// <summary>
    /// Perihal / subject surat.
    /// </summary>
    [Required(ErrorMessage = "Perihal wajib diisi.")]
    public string Perihal { get; set; } = string.Empty;

    //Undangan, Edaran, dll
    [MaxLength(50)]
    public string? KategoriSurat { get; set; }

    /// <summary>
    /// ID user yang mencatat surat (biasanya TU/Sekretaris yang login).
    /// </summary>
    [Required(ErrorMessage = "User ID pencatat wajib diisi.")]
    public int UserId { get; set; }

    // NomorAgenda tidak diisi manual — digenerate otomatis oleh sistem

    /// <summary>
    /// Nomor agenda dari preview (optional).
    /// Jika kosong, backend akan generate otomatis.
    /// Jika sudah dipakai, backend akan generate nomor baru.
    /// </summary>
    [MaxLength(100)]
    public string? NomorAgendaPreview { get; set; }
}