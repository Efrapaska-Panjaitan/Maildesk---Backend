using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDesk.API.Entities;

[Table("disposisi")]
public class Disposisi
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("surat_id")]
    public int SuratId { get; set; }

    [Column("pemberi_id")]
    public int PemberiId { get; set; }

    [Column("penerima_id")]
    public int PenerimaId { get; set; }

    [Column("tanggal_disposisi")]
    public DateOnly TanggalDisposisi { get; set; }

    [Column("sifat_disposisi")]
    [MaxLength(50)]
    public string SifatDisposisi { get; set; } = "Biasa"; // Biasa | Penting | ..

    [Column("instruksi")]
    public string? Instruksi { get; set; } 

    //default 'Pending'
    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    /*[Column("nomor_agenda")]
    [MaxLength(100)]
    public string? NomorAgenda { get; set; }
    */

    /*[Column("is_archived")]
    public bool IsArchived { get; set; } = false;
    */

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    //diisi saat penerima klik "Terima Disposisi"
    [Column("waktu_diterima")]
    public DateTime? WaktuDiterima { get; set; }

    //diisi saat penerima klik "Tugas Selesai"
    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [ForeignKey("SuratId")]
    public Surat Surat { get; set; } = null!;

    [ForeignKey("PemberiId")]
    public User Pemberi { get; set; } = null!;

    [ForeignKey("PenerimaId")]
    public User Penerima { get; set; } = null!;
}