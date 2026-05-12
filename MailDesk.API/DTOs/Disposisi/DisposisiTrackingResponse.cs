namespace MailDesk.API.DTOs.Disposisi;

public class DisposisiTrackingResponse
{
    // Info surat
    public int SuratId { get; set; }
    public string? NoSurat { get; set; }
    public string PerihalSurat { get; set; } = string.Empty;
    public string? NamaFile { get; set; }

    // Posisi terkini
    public string PosisiTerkini { get; set; } = string.Empty;   // Nama user pemegang terkini
    public string? RolePosisiTerkini { get; set; }
    public string StatusTerkini { get; set; } = string.Empty;   // Pending | Accepted | Completed

    // Timeline lengkap
    public List<TrackingStep> Riwayat { get; set; } = new();
}

public class TrackingStep
{
    public int StepOrder { get; set; }
    public int DisposisiId { get; set; }
    public string NamaPemberi { get; set; } = string.Empty;
    public string? RolePemberi { get; set; }
    public string NamaPenerima { get; set; } = string.Empty;
    public string? RolePenerima { get; set; }
    public string SifatDisposisi { get; set; } = string.Empty;
    public string? Instruksi { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Waktu { get; set; }

    // true = ini adalah posisi surat saat ini (ujung chain)
    public bool IsPosisiTerkini { get; set; }
}