using Sirenix.OdinInspector;
using System;

namespace YNL.Bases
{
    [System.Serializable]
    public class PlayerStats
    {
        [Title("Resources")]
        public uint Protein;
        public uint Plasma;
        public uint Antigen;
        public uint StemCell;
        public uint Enzyme;
        public uint DNA1;
        public uint DNA2;
        public uint DNA3;
        public uint DNA4;

        [Title("Levels")]
        public uint DPSLevel = 1;
        public uint MSLevel = 1;
        public uint CapacityLevel = 1;
        public uint RadiusLevel = 1;
        public uint TentacleLevel = 1;
        public uint HPLevel = 1;

        [Title("Attributes")]
        public float DPS;
        public float MS;
        public float Capacity;
        public float Radius;
        public float Tentacle;
        public float HP;

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
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

    }

    public enum ResourceType : byte { Protein, Plasma, Antigen, StemCell, Enzyme, DNA1, DNA2, DNA3, DNA4 }
    public enum AttributeType : byte { DPS, MS, Capacity, Radius, Tentacle, HP }
}