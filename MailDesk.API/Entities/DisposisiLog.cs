using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDesk.API.Entities;

[Table("disposisi_log")]
public class DisposisiLog
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>Disposisi yang mengalami perubahan.</summary>
    [Column("disposisi_id")]
    public int DisposisiId { get; set; }

    /// <summary>Surat terkait — disimpan langsung agar mudah query log per surat.</summary>
    [Column("surat_id")]
    public int SuratId { get; set; }

    /// <summary>User yang melakukan aksi (pemberi/penerima disposisi).</summary>
    [Column("user_id")]
    public int? UserId { get; set; }

    /// <summary>
    /// Jenis aksi yang dilakukan.
    /// Nilai: DIBUAT | DITERIMA | DISELESAIKAN
    /// </summary>
    [Column("aksi")]
    [MaxLength(50)]
    public string Aksi { get; set; } = string.Empty;

    /// <summary>Status disposisi sebelum aksi terjadi. Null saat disposisi baru dibuat.</summary>
    [Column("status_lama")]
    [MaxLength(50)]
    public string? StatusLama { get; set; }

    /// <summary>Status disposisi setelah aksi terjadi.</summary>
    [Column("status_baru")]
    [MaxLength(50)]
    public string? StatusBaru { get; set; }

    /// <summary>Catatan tambahan (opsional) dari user saat melakukan aksi.</summary>
    [Column("keterangan")]
    public string? Keterangan { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("DisposisiId")]
    public Disposisi Disposisi { get; set; } = null!;

    [ForeignKey("SuratId")]
    public Surat Surat { get; set; } = null!;

    [ForeignKey("UserId")]
    public User? User { get; set; }
}
