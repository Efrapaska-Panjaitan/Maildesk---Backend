namespace Maildesk.Api.Entities;

public class SuratKeluar
{
    public long Id { get; set; }

    public string NomorAgenda { get; set; } = string.Empty;

    public string NomorSurat { get; set; } = string.Empty;

    public DateOnly TanggalSurat { get; set; }

    public string TujuanSurat { get; set; } = string.Empty;

    public string? InstansiTujuan { get; set; }

    public string Perihal { get; set; } = string.Empty;

    public string? IsiRingkas { get; set; }

    public string? KodeKlasifikasi { get; set; }

    public string TingkatPrioritas { get; set; } = "normal";

    public string Status { get; set; } = "draft";

    public long DibuatOleh { get; set; }

    public long? DiperbaruiOleh { get; set; }

    public DateTime DibuatPada { get; set; }

    public DateTime DiperbaruiPada { get; set; }
}