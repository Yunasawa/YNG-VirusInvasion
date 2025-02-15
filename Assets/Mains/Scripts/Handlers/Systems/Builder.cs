using System;
using System.Globalization;
using Unity.VisualScripting;

public static class Builder
{
    public static string ReplaceStats(this string description, string stat1, string stat2)
    {
        return description.Replace("$1", stat1).Replace("$2", stat2);
    }

    public static string RoundValue(this int number)
    {
        if (number < 10000) return number.ToString(CultureInfo.InvariantCulture);

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
    public static string RoundValue(this uint number) => ((int)number).RoundValue();

    public static string UpgradeFieldText()
    {
        string text = "";

        return text;
    }

    public static string AddComma(this int number) => string.Format("{0:N0}", number);
    public static string AddComma(this string input) => string.Format("{0:N0}", input);
}
