using UnityEngine;

public static class Key
{
    public static class Config
    {
        public static bool EnableSound = true;
        public static bool EnableMusic = true;

        public const int FarmCountdown = 10;
        public const float TypeWriterSpeed = 0.02f;
        public static int TypeWritterDelay = Mathf.RoundToInt(TypeWriterSpeed * 1000);
    }
    public static class Path
    {
        public static string SaveFile = $"{Application.persistentDataPath}/GameData.json";
    }
    public static class Stats
    {
        public const string ExtraSpeed = "ExtraSpeed";
        public const string ExtraCapacity = "ExtraCapacity";
        public const string ExtraHP = "ExtraHP";
        public const string ExtraDPS = "ExtraDPS";
        public const string ExtraResources = "ExtraResources";
        public const string ExtraDefence = "ExtraDefence";
        public const string ExtraSlowdown = "ExtraSlowdown";

        public const string HunterCellAmount = "HunterCellAmount";
        public const string HunterCellDPS = "HunterCellDPS";
        public const string HunterCellCapacity = "HunterCellCapacity";
        public const string DeliveryCellAmount = "DeliveryCellAmount";
        public const string DeliveryCellCapacity = "DeliveryCellCapacity";

        public const string ExtraDPSBoost = "ExtraDPSBoost";
        public const string ExtraHPBoost = "ExtraHPBoost";
        public const string ExtraRegenBoost = "ExtraRegenBoost";
    }
}