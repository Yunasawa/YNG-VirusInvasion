using Unity.VisualScripting;

public static class Builder
{
    public static string ReplaceStats(this string description, string stat1, string stat2)
    {
        return description.Replace("$1", stat1).Replace("$2", stat2);
    }
}