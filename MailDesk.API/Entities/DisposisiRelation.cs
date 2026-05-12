using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDesk.API.Entities;

[Table("disposisi_relation")]
public class DisposisiRelation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("parent_id")]
    public int ParentId { get; set; }

    [Column("child_id")]
    public int ChildId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("ParentId")]
    public Disposisi Parent { get; set; } = null!;

    [ForeignKey("ChildId")]
    public Disposisi Child { get; set; } = null!;
}