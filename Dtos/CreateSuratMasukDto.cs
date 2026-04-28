using System.ComponentModel.DataAnnotations;

namespace Maildesk.Api.Dtos;

public class CreateSuratMasukDto
{
    [Required(ErrorMessage = "Nomor agenda wajib diisi.")]
    [StringLength(100)]
    public string NomorAgenda { get; set; } = string.Empty;

    [StringLength(100)]
    public string? NomorSurat { get; set; }

    public DateOnly? TanggalSurat { get; set; }

    [Required(ErrorMessage = "Tanggal diterima wajib diisi.")]
    public DateOnly TanggalDiterima { get; set; }

    [Required(ErrorMessage = "Nama pengirim wajib diisi.")]
    [StringLength(150)]
    public string NamaPengirim { get; set; } = string.Empty;

    [StringLength(150)]
    public string? InstansiPengirim { get; set; }

    [Required(ErrorMessage = "Perihal wajib diisi.")]
    [StringLength(200)]
    public string Perihal { get; set; } = string.Empty;

    public string? Deskripsi { get; set; }

    [StringLength(50)]
    public string? KodeKlasifikasi { get; set; }

    [StringLength(50)]
    public string? JenisSumber { get; set; }

    [Required(ErrorMessage = "Tingkat prioritas wajib diisi.")]
    [StringLength(50)]
    public string TingkatPrioritas { get; set; } = "normal";

    [Required(ErrorMessage = "Status wajib diisi.")]
    [StringLength(50)]
    public string Status { get; set; } = "diterima";
}