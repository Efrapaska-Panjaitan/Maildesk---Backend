using System.ComponentModel.DataAnnotations;

namespace MailDesk.API.DTOs.Inbox;

public class UpdateInboxStatusRequest
{
    private static readonly HashSet<string> _allowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Menunggu Tindakan",
        "Sudah Dibaca",
        "Belum Dibaca",
        "Sudah Didisposisikan"
    };

    [Required(ErrorMessage = "Status wajib diisi.")]
    public string Status { get; set; } = string.Empty;

    public bool IsValid() => _allowedStatuses.Contains(Status);

    public static string AllowedValues =>
        string.Join(", ", new[]
        {
            "Menunggu Tindakan",
            "Sudah Dibaca",
            "Belum Dibaca",
            "Sudah Didisposisikan"
        }.Select(s => $"\"{s}\""));
}
