using System.ComponentModel.DataAnnotations;

namespace Maildesk.Api.Dtos;

public class CreateSuratKeluarDto
{
    [StringLength(100)]
    public string? NoSurat { get; set; }

    [StringLength(100)]
    public string? NomorAgenda { get; set; }

    public DateOnly? TanggalSurat { get; set; }

    [Required(ErrorMessage = "Penerima surat wajib diisi.")]
    [StringLength(150)]
    public string Kepada { get; set; } = string.Empty;

    [Required(ErrorMessage = "Perihal wajib diisi.")]
    public string Perihal { get; set; } = string.Empty;

    public string? NamaFile { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "Draft";
}