using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDesk.API.Entities;

[Table("inbox")]
public class Inbox
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("surat_id")]
    public int? SuratId { get; set; }

    [Column("pengirim_id")]
    public int? PengirimId { get; set; }

    [Column("penerima_id")]
    public int? PenerimaId { get; set; }

    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "Menunggu Tindakan";

    [Column("catatan_pengantar")]
    public string? CatatanPengantar { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("SuratId")]
    public Surat? Surat { get; set; }

    [ForeignKey("PengirimId")]
    public User? Pengirim { get; set; }

    [ForeignKey("PenerimaId")]
    public User? Penerima { get; set; }
}