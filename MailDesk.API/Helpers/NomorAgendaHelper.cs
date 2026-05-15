using System.Text.RegularExpressions;

namespace MailDesk.API.Helpers;

public static class NomorAgendaHelper
{
    // Format: SM/YYYY/MM/XXX atau SK/YYYY/MM/XXX
    private static readonly Regex FormatRegex =
        new(@"^S[MK]/\d{4}/\d{2}/\d{3}$", RegexOptions.Compiled);

    /// <summary>
    /// Generate nomor agenda berdasarkan jenis surat.
    /// Contoh: SM/2026/05/001 (Masuk) atau SK/2026/05/001 (Keluar)
    /// </summary>
    public static string Generate(string jenisSurat, int urutan)
    {
        var prefix = jenisSurat == "Keluar" ? "SK" : "SM";
        var now = DateTime.UtcNow;
        return $"{prefix}/{now.Year}/{now.Month:D2}/{urutan:D3}";
    }

    /// <summary>
    /// Generate overload tanpa jenisSurat — default ke Surat Masuk (SM).
    /// </summary>
    public static string Generate(int urutan)
    {
        return Generate("Masuk", urutan);
    }

    /// <summary>
    /// Preview nomor agenda berikutnya (belum final).
    /// </summary>
    public static string Preview(int currentCount)
    {
        return Generate("Masuk", currentCount + 1);
    }

    /// <summary>
    /// Validasi format nomor agenda.
    /// Valid: SM/2026/05/001 atau SK/2026/05/001
    /// </summary>
    public static bool IsValidFormat(string? nomor)
    {
        if (string.IsNullOrWhiteSpace(nomor)) return false;
        return FormatRegex.IsMatch(nomor);
    }
}