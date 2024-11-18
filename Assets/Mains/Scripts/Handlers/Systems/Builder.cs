using Unity.VisualScripting;

public static class Builder
{
    public static string ReplaceStats(this string description, float stat1, float stat2)
    {
        return description.Replace("$1", stat1.ToString()).Replace("$2", stat2.ToString());
    }
}