namespace Maildesk.Api.Entities;

public class SuratMasuk
{
    public long Id { get; set; }
    public string NomorAgenda { get; set; } = null!;
    public string? NomorSurat { get; set; }
    public DateOnly? TanggalSurat { get; set; }
    public DateOnly TanggalDiterima { get; set; }
    public string NamaPengirim { get; set; } = null!;
    public string? InstansiPengirim { get; set; }
    public string Perihal { get; set; } = null!;
    public string? Deskripsi { get; set; }
    public string? KodeKlasifikasi { get; set; }
    public string? JenisSumber { get; set; }
    public string TingkatPrioritas { get; set; } = "normal";
    public string Status { get; set; } = "diterima";
    public DateTime DibuatPada { get; set; } = DateTime.UtcNow;
}