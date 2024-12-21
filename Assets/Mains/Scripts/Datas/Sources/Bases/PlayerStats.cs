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
            public int Level;
            public float Step;

            public float Value => Step * Level;
            public float NextValue => Step * (Level + 1);
        }

        public int CurrentLevel = 0;
        public int CurrentExp;

        public SerializableDictionary<ResourceType, float> Resources = new();

        public SerializableDictionary<AttributeType, float> Attributes = new();
        public SerializableDictionary<AttributeType, float> Bonuses = new();
        public SerializableDictionary<AttributeType, uint> Levels = new();

        public SerializableDictionary<string, ExtraStats> ExtraStatsLevel = new();
        public SerializableDictionary<string, SerializableDictionary<string, ExtraStats>> FarmStats = new();

        public void Reset()
        {
            CurrentLevel = CurrentExp = 0;

            Resources.Clear();
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType))) Resources.Add(type, 0);

            Bonuses.Clear();
            foreach (AttributeType type in Enum.GetValues(typeof(AttributeType))) Bonuses.Add(type, 0);

            Levels.Clear();
            foreach (AttributeType type in Enum.GetValues(typeof(AttributeType))) Levels.Add(type, 0);

            foreach (var pair in ExtraStatsLevel) ExtraStatsLevel[pair.Key].Level = 0;
            foreach (var pair in FarmStats)
            {
                foreach (var pair2 in pair.Value) pair.Value[pair2.Key].Level = 0;
            }
        }

        public void AdjustResources(ResourceType type, int amount)
        {
            if(amount >= 0) Resources[type] += (uint)amount;
            else
            {
                if (amount > Resources[type]) Resources[type] = 0;
                Resources[type] -= (uint)amount;
            }
        }

        public void UpdateAttributes(AttributeType type)
        {
            Attributes[type] = Formula.Stats.GetAttributeValue(type);
        }

        public void UpgradeLevels(AttributeType type)
        {
            Levels[type]++;
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