namespace MailDesk.API.Helpers;

public static class NomorAgendaHelper
{
    /// <summary>
    /// Generate nomor agenda format: SM/YYYY/MM/XXX
    /// Contoh: SM/2026/05/001
    /// </summary>
    public static string Generate(int urutanBulanIni)
    {
        var now = DateTime.Now;
        return Generate(now.Year, now.Month, urutanBulanIni);
    }

    public static string Generate(int year, int month, int urutan)
    {
        return $"SM/{year}/{month:D2}/{urutan:D3}";
    }

    /// <summary>
    /// Preview nomor agenda berikutnya (belum final)
    /// </summary>
    public static string Preview(int currentCount)
    {
        return Generate(currentCount + 1);
    }
}