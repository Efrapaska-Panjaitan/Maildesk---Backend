using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDesk.API.Entities;

[Table("tracking_disposisi")]
public class TrackingDisposisi
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("disposisi_id")]
    public int? DisposisiId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("status")]
    [MaxLength(50)]
    public string? Status { get; set; }

    [Column("catatan")]
    public string? Catatan { get; set; }

    [Column("waktu_update")]
    public DateTime WaktuUpdate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("DisposisiId")]
    public Disposisi? Disposisi { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}