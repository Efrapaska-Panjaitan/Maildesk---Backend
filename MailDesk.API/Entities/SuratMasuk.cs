using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDesk.API.Entities;

[Table("surat_masuk")]
public class SuratMasuk
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("no_surat")]
    [MaxLength(100)]
    public string NoSurat { get; set; } = string.Empty;

    [Column("nomor_agenda")]
    [MaxLength(100)]
    public string? NomorAgenda { get; set; }

    [Column("tanggal_surat")]
    public DateOnly TanggalSurat { get; set; }

    [Column("asal_pengirim")]
    [MaxLength(150)]
    public string AsalPengirim { get; set; } = string.Empty;

    [Column("perihal")]
    public string Perihal { get; set; } = string.Empty;

     // File lampiran — binary tidak dipakai (nullable)
    [Column("file_lampiran")]
    public byte[]? FileLampiran { get; set; }

    // Nama file asli dari pengirim
    [Column("nama_file")]
    [MaxLength(255)]
    public string? NamaFile { get; set; }

    // Path file di server — digunakan untuk upload PDF
    [Column("file_path")]
    [MaxLength(500)]
    public string? FilePath { get; set; }

    [Column("is_archived")]
    public bool IsArchived { get; set; } = false;

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey("UserId")]
    public User? User { get; set; }
}