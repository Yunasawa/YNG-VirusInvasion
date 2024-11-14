using Sirenix.OdinInspector;
using System;

namespace YNL.Bases
{
    [System.Serializable]
    public class PlayerStats
    {
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

        [FoldoutGroup("Extras")] public float ExtraDPS;
        [FoldoutGroup("Extras")] public float ExtraMS;
        [FoldoutGroup("Extras")] public float ExtraCapacity;
        [FoldoutGroup("Extras")] public float ExtraHP;
        [FoldoutGroup("Extras")] public float ExtraResources;
        [FoldoutGroup("Extras")] public float ExtraDefense;
        [FoldoutGroup("Extras")] public float ExtraSlow;

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
                case ResourceType.Protein: AdjustPositiveValue(ref Protein, amount); break;
                case ResourceType.Plasma: AdjustPositiveValue(ref Plasma, amount); break;
                case ResourceType.Antigen: AdjustPositiveValue(ref Antigen, amount); break;
                case ResourceType.StemCell: AdjustPositiveValue(ref StemCell, amount); break;
                case ResourceType.Enzyme: AdjustPositiveValue(ref Enzyme, amount); break;
                case ResourceType.DNA1: AdjustPositiveValue(ref DNA1, amount); break;
                case ResourceType.DNA2: AdjustPositiveValue(ref DNA2, amount); break;
                case ResourceType.DNA3: AdjustPositiveValue(ref DNA3, amount); break;
                case ResourceType.DNA4: AdjustPositiveValue(ref DNA4, amount); break;
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

    public enum ResourceType : byte { Protein, Plasma, Antigen, StemCell, Enzyme, DNA1, DNA2, DNA3, DNA4, Currency3 }
    public enum AttributeType : byte { DPS, MS, Capacity, Radius, Tentacle, HP }
}