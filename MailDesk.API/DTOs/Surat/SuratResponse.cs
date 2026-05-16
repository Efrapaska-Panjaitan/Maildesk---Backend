namespace MailDesk.API.DTOs.Surat;

public class SuratResponse
{
    public int Id { get; set; }
    public string? NoSurat { get; set; } 
    public string NomorAgenda { get; set; } = string.Empty;
    public string JenisSurat { get; set; } = string.Empty;
    public string? KategoriSurat { get; set; }
    public DateOnly TanggalSurat { get; set; }
    public string Pengirim { get; set; } = string.Empty;
    public string Penerima { get; set; } = string.Empty;
    public string Perihal { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsArchived { get; set; }

    /// <summary>Nama TU/Sekretaris yang mencatat surat.</summary>
    public string? PencatatNama { get; set; }

    /// <summary>Nama asli file PDF dari pengirim (untuk display di UI).</summary>
    public string? NamaFile { get; set; }

    /// <summary>URL lengkap untuk mengakses file PDF. Null jika belum ada lampiran.</summary>
    public string? FileUrl { get; set; }

    public bool HasLampiran { get; set; }
    public DateTime CreatedAt { get; set; }
}