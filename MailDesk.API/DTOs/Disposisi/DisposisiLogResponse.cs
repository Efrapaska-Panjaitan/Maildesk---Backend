namespace MailDesk.API.DTOs.Disposisi;

/// <summary>Satu entri riwayat perubahan status disposisi.</summary>
public class DisposisiLogResponse
{
    public int Id { get; set; }
    public int DisposisiId { get; set; }
    public int SuratId { get; set; }

    /// <summary>Nama user yang melakukan aksi.</summary>
    public string? NamaUser { get; set; }

    /// <summary>Jenis aksi: DIBUAT | DITERIMA | DISELESAIKAN</summary>
    public string Aksi { get; set; } = string.Empty;

    public string? StatusLama { get; set; }
    public string? StatusBaru { get; set; }
    public string? Keterangan { get; set; }

    public DateTime CreatedAt { get; set; }
}
