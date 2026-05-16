using System.ComponentModel.DataAnnotations;

namespace MailDesk.API.DTOs.Tracking;

public class UpdateSuratStatusRequest
{
    private static readonly HashSet<string> _allowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Baru",
        "Diproses",
        "Selesai"
    };

    [Required(ErrorMessage = "Status wajib diisi.")]
    public string Status { get; set; } = string.Empty;

    public bool IsValid() => _allowedStatuses.Contains(Status);

    public static string AllowedValues =>
        string.Join(", ", new[]
        {
            "Baru",
            "Diproses",
            "Selesai"
        }.Select(s => $"\"{s}\""));
}
