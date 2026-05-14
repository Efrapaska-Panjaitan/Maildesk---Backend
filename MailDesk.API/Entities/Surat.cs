using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDesk.API.Entities;

[Table("surat")]
public class Surat
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("no_surat")]
    [MaxLength(100)]
    public string? NoSurat { get; set; } 

    [Column("nomor_agenda")]
    [MaxLength(100)]
    public string? NomorAgenda { get; set; }

    //'Masuk' atau 'Keluar'
    [Column("jenis_surat")]
    [MaxLength(20)]
    public string JenisSurat { get; set; } = string.Empty;

    //Undangan, Edaran, dll
    [Column("kategori_surat")]
    [MaxLength(50)]
    public string? KategoriSurat { get; set; }

    [Column("tanggal_surat")]
    public DateOnly TanggalSurat { get; set; }

    [Column("pengirim")]
    [MaxLength(150)]
    public string Pengirim { get; set; } = string.Empty;

    [Column("penerima")]
    [MaxLength(150)]
    public string Penerima { get; set; } = string.Empty;

    [Column("perihal")]
    public string Perihal { get; set; } = string.Empty;

    //untuk OCR full-text search
    [Column("isi_teks_ocr")]
    public string? IsiTeksOcr { get; set; }

    [Column("file_lampiran")]
    public byte[]? FileLampiran { get; set; }

    /// <summary>Nama asli file PDF dari pengirim (untuk display di UI).</summary>
    [Column("nama_file")]
    [MaxLength(255)]
    public string? NamaFile { get; set; }

    /// <summary>
    /// Path relatif file PDF di disk storage.
    /// Format: uploads/surat/YYYY/MM/{uuid}.pdf
    /// Digunakan untuk konstruksi FileUrl di layer API.
    /// </summary>
    [Column("file_path")]
    [MaxLength(500)]
    public string? FilePath { get; set; }

    // default 'Baru'
    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "Baru";

    [Column("is_archived")]
    public bool IsArchived { get; set; } = false;

    /// <summary>
    /// ID TU/Sekretaris yang mencatat surat ini ke sistem.
    /// </summary>
    [Column("user_id")]
    public int? UserId { get; set; }

    /// <summary>
    /// ID Pimpinan/user yang dituju oleh TU/Sekretaris
    /// untuk membaca dan melakukan disposisi surat ini.
    /// </summary>
    [Column("ditujukan_ke_id")]
    public int? DitujukanKeId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    /// <summary>TU/Sekretaris pencatat surat.</summary>
    [ForeignKey("UserId")]
    public User? User { get; set; }

    /// <summary>Pimpinan/user tujuan disposisi yang dipilih oleh TU saat mencatat.</summary>
    [ForeignKey("DitujukanKeId")]
    public User? DitujukanKe { get; set; }
}