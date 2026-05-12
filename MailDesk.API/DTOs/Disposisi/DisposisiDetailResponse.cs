namespace MailDesk.API.DTOs.Disposisi;

public class DisposisiDetailResponse
{
    public int Id { get; set; }

    // Info Surat
    public int SuratId { get; set; }
    public string? NoSurat { get; set; }
    public string PerihalSurat { get; set; } = string.Empty;
    public string? NamaFile { get; set; }
    public bool HasLampiran { get; set; }

    // Pemberi
    public int PemberiId { get; set; }
    public string NamaPemberi { get; set; } = string.Empty;
    public string? RolePemberi { get; set; }

    // Penerima
    public int PenerimaId { get; set; }
    public string NamaPenerima { get; set; } = string.Empty;
    public string? RolePenerima { get; set; }

    public DateOnly TanggalDisposisi { get; set; }
    public string SifatDisposisi { get; set; } = string.Empty;
    public string? Instruksi { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? WaktuDiterima { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Relasi chain
    public int? ParentDisposisiId { get; set; }
}