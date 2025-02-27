using UnityEngine;
using YNL.Bases;

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
    public static class Resource
    {
        public static string GetName(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Food1: return "Protein";
                case ResourceType.Energy1: return "Plasma";
                case ResourceType.Energy2: return "Antigen";
                case ResourceType.Energy3: return "Stem Cell";
                case ResourceType.Material1: return "Enzyme";
                case ResourceType.Gen1: return "DNA 1";
                case ResourceType.Gen2: return "DNA 2";
                case ResourceType.Gen3: return "DNA 3";
                case ResourceType.Gen4: return "DNA 4";
                default: return "Gem";
            }
        }
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

        public const string HunterAmount = "HunterAmount";
        public const string HunterDPS = "HunterDPS";
        public const string HunterCapacity = "HunterCapacity";
        public const string DeliveryAmount = "DeliveryAmount";
        public const string DeliveryCapacity = "DeliveryCapacity";

        public const string ExtraDPSBoost = "ExtraDPSBoost";
        public const string ExtraHPBoost = "ExtraHPBoost";
        public const string ExtraRegenBoost = "ExtraRegenBoost";
    }
}