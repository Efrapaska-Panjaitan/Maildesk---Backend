using System.ComponentModel.DataAnnotations;

namespace Maildesk.Api.Dtos;

public class CreateSuratMasukDto
{
    [Required(ErrorMessage = "Nomor surat wajib diisi.")]
    [StringLength(100)]
    public string NoSurat { get; set; } = string.Empty;

    [StringLength(100)]
    public string? NomorAgenda { get; set; }

    [Required(ErrorMessage = "Tanggal surat wajib diisi.")]
    public DateOnly TanggalSurat { get; set; }

    [Required(ErrorMessage = "Asal pengirim wajib diisi.")]
    [StringLength(150)]
    public string AsalPengirim { get; set; } = string.Empty;

    [Required(ErrorMessage = "Perihal wajib diisi.")]
    public string Perihal { get; set; } = string.Empty;

    public string? NamaFile { get; set; }

    // FileLampiran dikirim terpisah via multipart/form-data, tidak di JSON body
}