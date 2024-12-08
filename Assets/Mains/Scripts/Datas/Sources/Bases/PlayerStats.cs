using Sirenix.OdinInspector;
using System;
using YNL.Utilities.Addons;

namespace YNL.Bases
{
    [System.Serializable]
    public class PlayerStats
    {
        [System.Serializable] public class ExtraStats
        {
            public uint Level;
            public float Step;

            public float Value => Step * Level;
            public float NextValue => Step * (Level + 1);
        }

        public SerializableDictionary<ResourceType, float> Resources = new();

        [FoldoutGroup("Levels")] public uint DPSLevel = 1;
        [FoldoutGroup("Levels")] public uint MSLevel = 1;
        [FoldoutGroup("Levels")] public uint CapacityLevel = 1;
        [FoldoutGroup("Levels")] public uint RadiusLevel = 1;
        [FoldoutGroup("Levels")] public uint TentacleLevel = 1;
        [FoldoutGroup("Levels")] public uint HPLevel = 1;

        [FoldoutGroup("Attributes")] public float DPS;
        [FoldoutGroup("Attributes")] public float MS;
        [FoldoutGroup("Attributes")] public float Capacity;
        [FoldoutGroup("Attributes")] public float Radius;
        [FoldoutGroup("Attributes")] public float Tentacle;
        [FoldoutGroup("Attributes")] public float HP;

        public SerializableDictionary<string, ExtraStats> ExtraStatsLevel = new();
        public SerializableDictionary<string, SerializableDictionary<string, ExtraStats>> FarmStats = new();

        public void AdjustResources(ResourceType type, int amount)
        {
            if(amount >= 0) Resources[type] += (uint)amount;
            else
            {
                if (amount > Resources[type]) Resources[type] = 0;
                Resources[type] -= (uint)amount;
            }
        }

    }

    [System.Serializable]
    public struct ResourcesInfo
    {
        public ResourceType Type;
        public uint Amount;
    }

    public enum ResourceType : byte { Food1, Energy1, Energy2, Energy3, Material1, Gen1, Gen2, Gen3, Gen4, Currency3 }
    public enum AttributeType : byte { DPS, MS, Capacity, Radius, Tentacle, HP }
}