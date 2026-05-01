using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDesk.API.Entities;

[Table("disposisi")]
public class Disposisi
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("surat_masuk_id")]
    public int? SuratMasukId { get; set; }

    [Column("pemberi_id")]
    public int? PemberiId { get; set; }

    [Column("penerima_id")]
    public int? PenerimaId { get; set; }

    [Column("tanggal_disposisi")]
    public DateOnly TanggalDisposisi { get; set; }

    [Column("sifat_disposisi")]
    [MaxLength(50)]
    public string? SifatDisposisi { get; set; }

    [Column("instruksi")]
    public string Instruksi { get; set; } = string.Empty;

    [Column("nomor_agenda")]
    [MaxLength(100)]
    public string? NomorAgenda { get; set; }

    [Column("is_archived")]
    public bool IsArchived { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("SuratMasukId")]
    public SuratMasuk? SuratMasuk { get; set; }

    [ForeignKey("PemberiId")]
    public User? Pemberi { get; set; }

    [ForeignKey("PenerimaId")]
    public User? Penerima { get; set; }
}