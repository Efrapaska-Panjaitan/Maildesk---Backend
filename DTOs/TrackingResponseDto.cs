namespace MailDesk.Api.DTOs;

public class TrackingResponseDto
{
    public int IdSurat { get; set; }
    public string? NoSurat { get; set; }
    public string? Perihal { get; set; }
    public string? StatusSurat { get; set; }
    
    public List<RiwayatItemDto> Timeline { get; set; } = new List<RiwayatItemDto>();
}

public class RiwayatItemDto
{
    public DateTime? Tanggal { get; set; }
    public string JenisAksi { get; set; } = string.Empty; 
    public string Keterangan { get; set; } = string.Empty;
    public string Dari { get; set; } = string.Empty;
    public string Kepada { get; set; } = string.Empty;
}