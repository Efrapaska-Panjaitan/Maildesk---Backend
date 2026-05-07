using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDesk.API.Entities;

[Table("template_surat")]
public class TemplateSurat
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nama_template")]
    [MaxLength(100)]
    public string NamaTemplate { get; set; } = string.Empty;

    [Column("isi_template")]
    public string IsiTemplate { get; set; } = string.Empty;

    [Column("dibuat_oleh")]
    public int? DibuatOleh { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("DibuatOleh")]
    public User? User { get; set; }
}