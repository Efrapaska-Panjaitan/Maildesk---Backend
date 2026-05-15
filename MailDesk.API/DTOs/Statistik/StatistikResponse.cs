namespace MailDesk.API.DTOs.Statistik;

public class StatistikResponse
{
    public int TotalSurat { get; set; }
    public List<LabelCountItem> SuratPerJenis { get; set; } = [];
    public List<LabelCountItem> SuratPerStatus { get; set; } = [];
    public int TotalDisposisi { get; set; }
    public List<LabelCountItem> DisposisiPerStatus { get; set; } = [];
}

public class LabelCountItem
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}
