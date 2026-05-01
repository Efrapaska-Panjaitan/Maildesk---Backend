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
        var tahun = now.Year.ToString();
        var bulan = now.Month.ToString("D2");  // 2 digit, e.g. "05"
        var urutan = urutanBulanIni.ToString("D3"); // 3 digit, e.g. "001"

        return $"SM/{tahun}/{bulan}/{urutan}";
    }
}