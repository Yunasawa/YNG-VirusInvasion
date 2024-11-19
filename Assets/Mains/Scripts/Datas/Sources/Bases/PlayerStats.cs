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

        [FoldoutGroup("Resources")] public uint Protein;
        [FoldoutGroup("Resources")] public uint Plasma;
        [FoldoutGroup("Resources")] public uint Antigen;
        [FoldoutGroup("Resources")] public uint StemCell;
        [FoldoutGroup("Resources")] public uint Enzyme;
        [FoldoutGroup("Resources")] public uint DNA1;
        [FoldoutGroup("Resources")] public uint DNA2;
        [FoldoutGroup("Resources")] public uint DNA3;
        [FoldoutGroup("Resources")] public uint DNA4;
        [FoldoutGroup("Resources")] public uint Currency3;

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

        private void AdjustPositiveValue(ref uint value, int addition)
        {
            if (addition >= 0) value += (uint)addition;
            else
            {
                if (addition > value) value = 0;
                value -= (uint)addition;
            }
        }
        public void AdjustResources(ResourceType type, int amount)
        {
            switch (type)
            {
                case ResourceType.Food1: AdjustPositiveValue(ref Protein, amount); break;
                case ResourceType.Energy1: AdjustPositiveValue(ref Plasma, amount); break;
                case ResourceType.Energy2: AdjustPositiveValue(ref Antigen, amount); break;
                case ResourceType.Energy3: AdjustPositiveValue(ref StemCell, amount); break;
                case ResourceType.Material1: AdjustPositiveValue(ref Enzyme, amount); break;
                case ResourceType.Gen1: AdjustPositiveValue(ref DNA1, amount); break;
                case ResourceType.Gen2: AdjustPositiveValue(ref DNA2, amount); break;
                case ResourceType.Gen3: AdjustPositiveValue(ref DNA3, amount); break;
                case ResourceType.Gen4: AdjustPositiveValue(ref DNA4, amount); break;
                case ResourceType.Currency3: AdjustPositiveValue(ref Currency3, amount); break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
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