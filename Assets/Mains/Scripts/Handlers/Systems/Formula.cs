using System;
using System.Collections.Generic;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;
using static UnityEngine.EventSystems.EventTrigger;
using BaseStats = YNL.Bases.PlayerStats;

public static class Formula
{
    private static BaseStats _stats => Game.Data.PlayerStats;
    private static SerializableDictionary<AttributeType, float> _bonus => Game.Data.PlayerStats.Bonuses;
    private static SerializableDictionary<string, BaseStats.ExtraStats> _extras => Game.Data.PlayerStats.ExtraStatsLevel;

    public static class Stats
    {
        public static uint GetMaxAttributeLevel(AttributeType type)
        {
            switch (type)
            {
                case AttributeType.DPS: case AttributeType.Capacity: case AttributeType.Radius: case AttributeType.HP:
                    return 100;
                case AttributeType.MS:
                    return 50;
                case AttributeType.Tentacle: case AttributeType.MultiAttack:
                    return 10;
                default: return 0;
            }
        }
        public static int GetMaxExtraLevel(string extra)
        {
            switch (extra)
            {
                case Key.Stats.DeliveryAmount: case Key.Stats.HunterAmount:
                    return 10;
                default: return 50;
            }
        }
        public static List<(ResourceType, int)> GetDPSRequirement()
        {
            uint level = _stats.Levels[AttributeType.DPS];

            List<(ResourceType, int)> requirements = new();

            float multiplier = level > 50 ? 0.15f : 0.2f;
            requirements.Add(new(ResourceType.Food1, Mathf.CeilToInt(23 * Mathf.Pow(1 + multiplier, level))));

            if (level >= 50 && level < 70)
            {
                uint lowLevel = level - 50;
                requirements.Add(new(ResourceType.Energy1, (int)Mathf.Min(14, lowLevel / 2 + 1 + level % 10 * 2)));
            }
            else if (level >= 70 && level < 90)
            {
                uint lowLevel = level - 70;
                requirements.Add(new(ResourceType.Gen1, (int)Mathf.Min(6, lowLevel / 2 + 1 + level % 10 * 2)));
            }
            else if (level >= 90 && level < 100)
            {
                requirements.Add(new(ResourceType.Gen1, 8 + (int)level % 10 * 2));
            }
            else if (level > 100)
            {
                requirements.Add(new(ResourceType.Gen1, 10));
            }
            return requirements;
        }
        public static List<(ResourceType, int)> GetMSRequirement()
        {
            uint level = _stats.Levels[AttributeType.MS];

            List<(ResourceType, int)> requirements = new();
            requirements.Add(new(ResourceType.Food1, Mathf.CeilToInt(23 * Mathf.Pow(1 + 0.22f, level))));

            if (level >= 35 && level < 40)
            {
                uint lowLevel = level - 35;
                requirements.Add(new(ResourceType.Gen1, (int)Mathf.Min(4, lowLevel + 1)));
                requirements.Add(new(ResourceType.Gen2, (int)Mathf.Min(4, lowLevel + 1)));
            }
            else if (level >= 40 && level < 50)
            {
                uint lowLevel = level - 40;
                requirements.Add(new(ResourceType.Gen1, (int)Mathf.Min(8, lowLevel + 4)));
                requirements.Add(new(ResourceType.Gen4, (int)Mathf.Min(10, lowLevel + 4)));
            }
            else if (level > 50)
            {
                uint lowLevel = level - 50;
                requirements.Add(new(ResourceType.Gen4, (int)Mathf.Min(16, lowLevel + 8)));
            }
            return requirements;
        }
        public static List<(ResourceType, int)> GetCapacityRequirement()
        {
            uint level = _stats.Levels[AttributeType.Capacity];

            List<(ResourceType, int)> requirements = new();

            float multiplier = level > 50 ? 0.15f : 0.2f;
            requirements.Add(new(ResourceType.Food1, (int)(25 * Math.Pow(1 + multiplier, level))));

            if (level >= 30 && level < 40)
            {
                uint lowLevel = level - 30;
                requirements.Add(new(ResourceType.Gen1, (int)Mathf.Min(6, lowLevel + 5)));
                requirements.Add(new(ResourceType.Gen2, (int)Mathf.Min(10, lowLevel + 8)));
            }
            else if (level >= 40 && level < 50)
            {
                uint lowLevel = level - 40;
                requirements.Add(new(ResourceType.Gen1, (int)Mathf.Min(6, lowLevel + 2)));
                requirements.Add(new(ResourceType.Gen3, (int)Mathf.Min(10, lowLevel + 8)));
            }
            else if (level > 50)
            {
                uint lowLevel = level - 50;
                requirements.Add(new(ResourceType.Gen3, (int)Mathf.Min(6, lowLevel + 3)));
                requirements.Add(new(ResourceType.Gen4, (int)Mathf.Min(10, lowLevel + 3)));
            }
            return requirements;
        }
        public static List<(ResourceType, int)> GetRadiusRequirement()
        {
            List<(ResourceType, int)> requirements = new();
            requirements.Add(new(ResourceType.Food1, Mathf.CeilToInt(500 * Mathf.Pow(1 + 0.03f, _stats.Levels[AttributeType.Radius]))));

            if (_stats.Levels[AttributeType.Radius] >= 30 && _stats.Levels[AttributeType.Radius] < 50)
            {
                uint lowLevel = _stats.Levels[AttributeType.Radius] - 30;
                requirements.Add(new(ResourceType.Gen1, (int)(lowLevel / 5 * 2) + 2));
            }
            else if (_stats.Levels[AttributeType.Radius] >= 50 && _stats.Levels[AttributeType.Radius] < 70)
            {
                uint lowLevel = _stats.Levels[AttributeType.Radius] - 50;
                if (lowLevel % 2 != 0)
                {
                    requirements.Add(new(ResourceType.Gen1, (int)lowLevel / 2 + 2));
                    requirements.Add(new(ResourceType.Gen4, (int)lowLevel / 2 + 2));
                }
            }
            else if (_stats.Levels[AttributeType.Radius] > 70)
            {
                uint lowLevel = _stats.Levels[AttributeType.Radius] - 70;
                if (lowLevel % 2 != 0)
                {
                    requirements.Add(new(ResourceType.Gen2, (int)Mathf.Min(6, lowLevel / 2 + 3)));
                    requirements.Add(new(ResourceType.Gen4, (int)Mathf.Min(10, lowLevel / 2 + 3)));
                }
            }
            return requirements;
        }
        public static List<(ResourceType, int)> GetTentacleRequirement()
        {
            List<(ResourceType, int)> requirements = new();

            switch (_stats.Levels[AttributeType.Tentacle])
            {
                case 0:
                    requirements.Add(new(ResourceType.Food1, 250));
                    break;
                case 1:
                    requirements.Add(new(ResourceType.Food1, 1000));
                    break;
                case 2:
                    requirements.Add(new(ResourceType.Food1, 2500));
                    requirements.Add(new(ResourceType.Gen1, 2));
                    break;
                case 3:
                    requirements.Add(new(ResourceType.Food1, 8000));
                    requirements.Add(new(ResourceType.Gen1, 3));
                    break;
                case 4:
                    requirements.Add(new(ResourceType.Food1, 22000));
                    requirements.Add(new(ResourceType.Gen1, 4));
                    break;
                case 5:
                    requirements.Add(new(ResourceType.Food1, 66000));
                    requirements.Add(new(ResourceType.Gen1, 4));
                    requirements.Add(new(ResourceType.Gen2, 2));
                    break;
                case 6:
                    requirements.Add(new(ResourceType.Food1, 198000));
                    requirements.Add(new(ResourceType.Gen1, 5));
                    requirements.Add(new(ResourceType.Gen4, 2));
                    break;
                case 7:
                    requirements.Add(new(ResourceType.Food1, 594000));
                    requirements.Add(new(ResourceType.Gen1, 6));
                    requirements.Add(new(ResourceType.Gen4, 2));
                    break;
                case 8:
                    requirements.Add(new(ResourceType.Food1, 1780000));
                    requirements.Add(new(ResourceType.Gen1, 4));
                    requirements.Add(new(ResourceType.Gen3, 7));
                    break;
                case 9:
                    requirements.Add(new(ResourceType.Food1, 5300000));
                    requirements.Add(new(ResourceType.Gen1, 5));
                    requirements.Add(new(ResourceType.Gen3, 8));
                    break;
            }

            return requirements;
        }
        public static List<(ResourceType, int)> GetHPRequirement()
        {
            List<(ResourceType, int)> requirements = new();
            requirements.Add(new(ResourceType.Food1, Mathf.CeilToInt(80 * Mathf.Pow(1 + 0.15f, _stats.Levels[AttributeType.HP]))));

            if (_stats.Levels[AttributeType.HP] >= 30 && _stats.Levels[AttributeType.HP] < 50)
            {
                uint lowLevel = _stats.Levels[AttributeType.HP] - 30;
                requirements.Add(new(ResourceType.Gen1, (int)(lowLevel / 5 * 2) + 2));
            }
            else if (_stats.Levels[AttributeType.HP] >= 50 && _stats.Levels[AttributeType.HP] < 70)
            {
                uint lowLevel = _stats.Levels[AttributeType.HP] - 50;
                if (lowLevel % 2 != 0)
                {
                    requirements.Add(new(ResourceType.Gen1, (int)lowLevel / 2 + 2));
                    requirements.Add(new(ResourceType.Gen4, (int)lowLevel / 2 + 2));
                }
            }
            else if (_stats.Levels[AttributeType.HP] > 70)
            {
                uint lowLevel = _stats.Levels[AttributeType.HP] - 70;
                if (lowLevel % 2 != 0)
                {
                    requirements.Add(new(ResourceType.Gen2, (int)Mathf.Min(6, lowLevel / 2 + 3)));
                    requirements.Add(new(ResourceType.Gen4, (int)Mathf.Min(10, lowLevel / 2 + 3)));
                }
            }
            return requirements;
        }
        public static List<(ResourceType, int)> GetGroupAttackRequirement()
        {
            List<(ResourceType, int)> requirements = new();

            switch (_stats.Levels[AttributeType.MultiAttack])
            {
                case 0:
                    requirements.Add(new(ResourceType.Food1, 250));
                    break;
                case 1:
                    requirements.Add(new(ResourceType.Food1, 1000));
                    break;
                case 2:
                    requirements.Add(new(ResourceType.Food1, 2500));
                    requirements.Add(new(ResourceType.Gen1, 2));
                    break;
                case 3:
                    requirements.Add(new(ResourceType.Food1, 8000));
                    requirements.Add(new(ResourceType.Gen1, 3));
                    break;
                case 4:
                    requirements.Add(new(ResourceType.Food1, 22000));
                    requirements.Add(new(ResourceType.Gen1, 4));
                    break;
                case 5:
                    requirements.Add(new(ResourceType.Food1, 66000));
                    requirements.Add(new(ResourceType.Gen1, 4));
                    requirements.Add(new(ResourceType.Gen2, 2));
                    break;
                case 6:
                    requirements.Add(new(ResourceType.Food1, 198000));
                    requirements.Add(new(ResourceType.Gen1, 5));
                    requirements.Add(new(ResourceType.Gen4, 2));
                    break;
                case 7:
                    requirements.Add(new(ResourceType.Food1, 594000));
                    requirements.Add(new(ResourceType.Gen1, 6));
                    requirements.Add(new(ResourceType.Gen4, 2));
                    break;
                case 8:
                    requirements.Add(new(ResourceType.Food1, 1780000));
                    requirements.Add(new(ResourceType.Gen1, 4));
                    requirements.Add(new(ResourceType.Gen3, 7));
                    break;
                case 9:
                    requirements.Add(new(ResourceType.Food1, 5300000));
                    requirements.Add(new(ResourceType.Gen1, 5));
                    requirements.Add(new(ResourceType.Gen3, 8));
                    break;
            }

            return requirements;
        }

        public static List<(ResourceType, int)> GetAttributeRequirement(AttributeType type)
        {
            if (type == AttributeType.DPS) return GetDPSRequirement();
            else if (type == AttributeType.MS) return GetMSRequirement();
            else if (type == AttributeType.Capacity) return GetCapacityRequirement();
            else if (type == AttributeType.Radius) return GetRadiusRequirement();
            else if (type == AttributeType.Tentacle) return GetTentacleRequirement();
            else if (type == AttributeType.HP) return GetHPRequirement();
            else if (type == AttributeType.MultiAttack) return GetGroupAttackRequirement();
            return null;
        }


        public static float GetDPS(uint level = 0)
        {
            level = level == 0 ? _stats.Levels[AttributeType.DPS] : level;
            int dps = 16;
            uint a = level / 10;
            uint b = level % 10;

            for (int i = 1; i <= a; i++) dps += i * 10;
            dps += (int)(b * (a + 1));

            return dps * (1 + _bonus[AttributeType.DPS].Percent()) + _extras[Key.Stats.ExtraDPS].Value;
        }
        public static float GetMS(uint level = 0)
        {
            level = level == 0 ? _stats.Levels[AttributeType.MS] : level;
            float baseValue = 16;

            if (level <= 20) return baseValue + level / 2 + _extras[Key.Stats.ExtraSpeed].Value;
            else
            {
                uint extraLevels = level - 20;
                return baseValue + 10 + extraLevels * 0.4f + _extras[Key.Stats.ExtraSpeed].Value;
            }
        }
        public static float GetCapacity(uint level = 0)
        {
            level = level == 0 ? _stats.Levels[AttributeType.Capacity] : level;
            float baseValue = 8;

            if (level < 25)
            {
                return baseValue + level * 5;
            }
            else if (level >= 25 && level < 50)
            {
                uint extraLevels = level - 25;
                return baseValue = 25 * 10 + (extraLevels) * 10 + _extras[Key.Stats.ExtraCapacity].Value;
            }
            else if (level >= 50 && level < 75)
            {
                uint extraLevels = level - 50;
                return baseValue = 25 * 20 + (extraLevels) * 20 + _extras[Key.Stats.ExtraCapacity].Value;
            }
            else
            {
                uint extraLevels = level - 75;
                return baseValue = 25 * 30 + (extraLevels) * 30 + _extras[Key.Stats.ExtraCapacity].Value;
            }
        }
        public static float GetRadius(uint level = 0)
        {
            level = level == 0 ? _stats.Levels[AttributeType.Radius] : level;
            float baseValue = 15f;

            if (level <= 20) return baseValue + level * 0.5f;
            else
            {
                uint extraLevels = level - 20;
                return baseValue + 20 * 0.5f + (extraLevels / 2) * 0.2f + ((extraLevels + 1) / 2) * 0.3f;
            }
        }
        public static float GetTentacle(uint level = 0)
        {
            level = level == 0 ? _stats.Levels[AttributeType.Tentacle] : level;
            return 1 + level;
        }
        public static int GetHP(uint level = 0)
        {
            level = level == 0 ? _stats.Levels[AttributeType.HP] : level;
            int baseValue = 22;
            return Mathf.RoundToInt(baseValue + (level / 2) * 2 + ((level + 1) / 2) * 3 + _extras[Key.Stats.ExtraHP].Value);
        }
        public static float GetGroupAttack(uint level = 0)
        {
            level = level == 0 ? _stats.Levels[AttributeType.MultiAttack] : level;
            return 1 + level;
        }

        public static float GetAttributeValue(AttributeType type, uint level = 0)
        {
            if (type == AttributeType.DPS) return GetDPS(level);
            else if (type == AttributeType.MS) return GetMS(level);
            else if (type == AttributeType.Capacity) return GetCapacity(level); 
            else if (type == AttributeType.Radius) return GetRadius(level);
            else if (type == AttributeType.Tentacle) return GetTentacle(level); 
            else if (type == AttributeType.HP) return GetHP(level);
            else if (type == AttributeType.MultiAttack) return GetGroupAttack(level);
            return 0;
        }


        public static float GetDamage(float damage)
        {
            return damage - _extras[Key.Stats.ExtraDefence].Value;
        }
        public static int GetResource(float amount)
        {
            return Mathf.RoundToInt(amount * (1 + _extras[Key.Stats.ExtraResources].Value.Percent()));
        }
        public static int GetHeal()
        {
            return Mathf.FloorToInt(Game.Data.PlayerStats.ExtraStatsLevel[Key.Stats.ExtraRegenBoost].Value);
        }
        public static float EnemyRadius => GetRadius() / 5;
    }
    public static class Value
    {
        public static float GetEnemyPullingSpeedMultiplier(Vector3 enemyPosition)
        {
            float distance = Vector3.Distance(enemyPosition, Player.Transform.position);
            if (distance >= 5) return distance / 3.33f;
            else return 1.5f;
        }
        public static Vector2 GetRandom2DPosition(float xRange, float yRange)
        {
            float x = UnityEngine.Random.Range(-xRange, xRange);
            float y = UnityEngine.Random.Range(-yRange, yRange);
            return new(x, y);
        }
    }
    public static class Upgrade
    {
        public static (ResourceType, int) GetMarketUpgradeCost(MarketStatsNode node, int level)
        {
            (ResourceType Type, int Amount) cost = new();

            cost.Type = node.BaseCost.Type;
            if (node.PercentCost) cost.Amount = Mathf.RoundToInt(node.BaseCost.Amount * Mathf.Pow(1 + node.StepCost.Percent(), level - 1));
            else cost.Amount = Mathf.RoundToInt(node.BaseCost.Amount + node.StepCost * level);

            return cost;
        }
        public static (ResourceType, int)[] GetMarketEvoluteCost(int evolution)
        {
            return new (ResourceType, int)[] { new(ResourceType.Gen1, 100) };
        }
        public static (ResourceType, int) GetFarmUpgradeCost(FarmStatsNode node, int level)
        {
            (ResourceType Type, int Amount) cost = new();

            cost.Type = node.BaseCost.Type;
            cost.Amount = Mathf.RoundToInt(node.BaseCost.Amount * Mathf.Pow(1 + node.StepCost.Percent(), level));

            return cost;
        }
    }
    public static class Level
    {
        public static int GetMaxExp(int level = -1)
        {
            level = level == -1 ? Game.Data.PlayerStats.CurrentLevel : level;

            switch (level)
            {
                case 0: return 4;
                case 1: return 80;
                case 2: return 1000;
                case 3: return 7000;
                case 4: return 32000;
                case 5: return 90000;
                case 6: return 380000;
                case 7: return 1500000;
                case 8: return 6000000;
                case 9: return 25000000;
                case 10: return 100000000;
                default: return int.MaxValue;
            }
        }
    }
}