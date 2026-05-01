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

    // file_lampiran & nama_file ditangani di task berikutnya (upload PDF)
    [Column("file_lampiran")]
    public byte[]? FileLampiran { get; set; }

    [Column("nama_file")]
    [MaxLength(255)]
    public string? NamaFile { get; set; }

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