using System;
using System.Globalization;
using Unity.VisualScripting;

public static class Builder
{
    public static string ReplaceStats(this string description, string stat1, string stat2)
    {
        return description.Replace("$1", stat1).Replace("$2", stat2);
    }

    public static string RoundValue(this uint number)
    {
        if (number < 1000) return number.ToString(CultureInfo.InvariantCulture);

        int digitGroups = (int)(Math.Log10(number) / 3);
        double formattedNumber = Math.Floor(number / Math.Pow(1000, digitGroups) * 100) / 100;

        string suffix = digitGroups switch
        {
            1 => "K",
            2 => "M",
            3 => "B",
            4 => "T",
            _ => ""
        };

        return $"{formattedNumber.ToString("0.##", CultureInfo.InvariantCulture)}{suffix}";
    }
}
