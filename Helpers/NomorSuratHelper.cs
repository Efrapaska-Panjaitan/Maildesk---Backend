namespace Maildesk.Api.Helpers;

public static class NomorSuratHelper
{
    public static string GenerateNomorAgenda(int urutan, DateTime tanggal)
    {
        return $"AGENDA/{urutan:000}/{GetBulanRomawi(tanggal.Month)}/{tanggal.Year}";
    }

    public static string GenerateNomorSurat(int urutan, string kodeUnit, DateTime tanggal)
    {
        return $"{urutan:000}/MD/{kodeUnit.ToUpper()}/{GetBulanRomawi(tanggal.Month)}/{tanggal.Year}";
    }

    private static string GetBulanRomawi(int bulan)
    {
        return bulan switch
        {
            1 => "I",
            2 => "II",
            3 => "III",
            4 => "IV",
            5 => "V",
            6 => "VI",
            7 => "VII",
            8 => "VIII",
            9 => "IX",
            10 => "X",
            11 => "XI",
            12 => "XII",
            _ => throw new ArgumentOutOfRangeException(nameof(bulan), "Bulan tidak valid")
        };
    }
}