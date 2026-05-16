namespace MailDesk.API.DTOs.User;

/// <summary>
/// Response ringkas untuk keperluan dropdown pemilihan user di UI.
/// Hanya berisi data minimal yang dibutuhkan frontend.
/// </summary>
public class UserDropdownResponse
{
    public int Id { get; set; }

    /// <summary>Nama lengkap user — ditampilkan sebagai label di dropdown</summary>
    public string Nama { get; set; } = string.Empty;

    /// <summary>Role user (misal: "Pimpinan") — untuk informasi tambahan di UI</summary>
    public string? NamaRole { get; set; }
}
