using System.Collections.Generic;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;
using BaseStats = YNL.Bases.PlayerStats;

public static class Formula
{
    private static BaseStats _stats => Game.Data.PlayerStats;

    public static class Stats
    {
        public static List<(ResourceType, uint)> GetDPSRequirement()
        {
            List<(ResourceType, uint)> requirements = new();
            requirements.Add(new(ResourceType.Food1, (uint)Mathf.CeilToInt(23 * Mathf.Pow(1 + 0.09f, _stats.DPSLevel))));

            if (_stats.DPSLevel >= 50 && _stats.DPSLevel < 70)
            {
                uint lowLevel = _stats.DPSLevel - 50;
                requirements.Add(new(ResourceType.Energy1, (uint)Mathf.Min(14, lowLevel / 2 + 1)));
            }
            else if (_stats.DPSLevel >= 70 && _stats.DPSLevel < 90)
            {
                uint lowLevel = _stats.DPSLevel - 70;
                requirements.Add(new(ResourceType.Gen1, (uint)Mathf.Min(6, lowLevel / 2 + 1)));
            }
            else if (_stats.DPSLevel >= 90 && _stats.DPSLevel < 100)
            {
                requirements.Add(new(ResourceType.Gen1, 8));
            }
            else if (_stats.DPSLevel > 100)
            {
                requirements.Add(new(ResourceType.Gen1, 10));
            }
            return requirements;
        }
        public static List<(ResourceType, uint)> GetMSRequirement()
        {
            List<(ResourceType, uint)> requirements = new();
            requirements.Add(new(ResourceType.Food1, (uint)Mathf.CeilToInt(23 * Mathf.Pow(1 + 0.2f, _stats.MSLevel))));

            if (_stats.MSLevel >= 35 && _stats.MSLevel < 40)
            {
                uint lowLevel = _stats.MSLevel - 35;
                requirements.Add(new(ResourceType.Gen1, (uint)Mathf.Min(4, lowLevel + 1)));
                requirements.Add(new(ResourceType.Gen2, (uint)Mathf.Min(4, lowLevel + 1)));
            }
            else if (_stats.MSLevel >= 40 && _stats.MSLevel < 50)
            {
                uint lowLevel = _stats.MSLevel - 40;
                requirements.Add(new(ResourceType.Gen1, (uint)Mathf.Min(8, lowLevel + 4)));
                requirements.Add(new(ResourceType.Gen4, (uint)Mathf.Min(10, lowLevel + 4)));
            }
            else if (_stats.MSLevel > 50)
            {
                uint lowLevel = _stats.MSLevel - 50;
                requirements.Add(new(ResourceType.Gen4, (uint)Mathf.Min(16, lowLevel + 8)));
            }
            return requirements;
        }
        public static List<(ResourceType, uint)> GetCapacityRequirement()
        {
            List<(ResourceType, uint)> requirements = new();
            requirements.Add(new(ResourceType.Food1, (uint)Mathf.CeilToInt(25 * Mathf.Pow(1 + 0.25f, _stats.CapacityLevel))));

            if (_stats.CapacityLevel >= 30 && _stats.CapacityLevel < 40)
            {
                uint lowLevel = _stats.CapacityLevel - 30;
                requirements.Add(new(ResourceType.Gen1, (uint)Mathf.Min(6, lowLevel + 5)));
                requirements.Add(new(ResourceType.Gen2, (uint)Mathf.Min(10, lowLevel + 8)));
            }
            else if (_stats.CapacityLevel >= 40 && _stats.CapacityLevel < 50)
            {
                uint lowLevel = _stats.CapacityLevel - 40;
                requirements.Add(new(ResourceType.Gen1, (uint)Mathf.Min(6, lowLevel + 2)));
                requirements.Add(new(ResourceType.Gen3, (uint)Mathf.Min(10, lowLevel + 8)));
            }
            else if (_stats.CapacityLevel > 50)
            {
                uint lowLevel = _stats.CapacityLevel - 50;
                requirements.Add(new(ResourceType.Gen3, (uint)Mathf.Min(6, lowLevel + 3)));
                requirements.Add(new(ResourceType.Gen4, (uint)Mathf.Min(10, lowLevel + 3)));
            }
            return requirements;
        }
        public static List<(ResourceType, uint)> GetRadiusRequirement()
        {
            List<(ResourceType, uint)> requirements = new();
            requirements.Add(new(ResourceType.Food1, (uint)Mathf.CeilToInt(500 * Mathf.Pow(1 + 0.03f, _stats.RadiusLevel))));

            if (_stats.RadiusLevel >= 30 && _stats.RadiusLevel < 50)
            {
                uint lowLevel = _stats.RadiusLevel - 30;
                requirements.Add(new(ResourceType.Gen1, (lowLevel / 5 * 2) + 2));
            }
            else if (_stats.RadiusLevel >= 50 && _stats.RadiusLevel < 70)
            {
                uint lowLevel = _stats.RadiusLevel - 50;
                if (lowLevel % 2 != 0)
                {
                    requirements.Add(new(ResourceType.Gen1, lowLevel / 2 + 2));
                    requirements.Add(new(ResourceType.Gen4, lowLevel / 2 + 2));
                }
            }
            else if (_stats.RadiusLevel > 70)
            {
                uint lowLevel = _stats.RadiusLevel - 70;
                if (lowLevel % 2 != 0)
                {
                    requirements.Add(new(ResourceType.Gen2, (uint)Mathf.Min(6, lowLevel / 2 + 3)));
                    requirements.Add(new(ResourceType.Gen4, (uint)Mathf.Min(10, lowLevel / 2 + 3)));
                }
            }
            return requirements;
        }
        public static List<(ResourceType, uint)> GetTentacleRequirement()
        {
            List<(ResourceType, uint)> requirements = new();

            switch (_stats.TentacleLevel)
            {
                case 1:
                    requirements.Add(new(ResourceType.Food1, 250));
                    break;
                case 2:
                    requirements.Add(new(ResourceType.Food1, 1000));
                    break;
                case 3:
                    requirements.Add(new(ResourceType.Food1, 2500));
                    requirements.Add(new(ResourceType.Gen1, 2));
                    break;
                case 4:
                    requirements.Add(new(ResourceType.Food1, 8000));
                    requirements.Add(new(ResourceType.Gen1, 3));
                    break;
                case 5:
                    requirements.Add(new(ResourceType.Food1, 22000));
                    requirements.Add(new(ResourceType.Gen1, 4));
                    break;
                case 6:
                    requirements.Add(new(ResourceType.Food1, 66000));
                    requirements.Add(new(ResourceType.Gen1, 4));
                    requirements.Add(new(ResourceType.Gen2, 2));
                    break;
                case 7:
                    requirements.Add(new(ResourceType.Food1, 198000));
                    requirements.Add(new(ResourceType.Gen1, 5));
                    requirements.Add(new(ResourceType.Gen4, 2));
                    break;
                case 8:
                    requirements.Add(new(ResourceType.Food1, 594000));
                    requirements.Add(new(ResourceType.Gen1, 6));
                    requirements.Add(new(ResourceType.Gen4, 2));
                    break;
                case 9:
                    requirements.Add(new(ResourceType.Food1, 1780000));
                    requirements.Add(new(ResourceType.Gen1, 4));
                    requirements.Add(new(ResourceType.Gen3, 7));
                    break;
                case 10:
                    requirements.Add(new(ResourceType.Food1, 5300000));
                    requirements.Add(new(ResourceType.Gen1, 5));
                    requirements.Add(new(ResourceType.Gen3, 8));
                    break;
            }

            return requirements;
        }
        public static List<(ResourceType, uint)> GetHPRequirement()
        {
            List<(ResourceType, uint)> requirements = new();
            requirements.Add(new(ResourceType.Food1, (uint)Mathf.CeilToInt(80 * Mathf.Pow(1 + 0.15f, _stats.HPLevel))));

            if (_stats.HPLevel >= 30 && _stats.HPLevel < 50)
            {
                uint lowLevel = _stats.HPLevel - 30;
                requirements.Add(new(ResourceType.Gen1, (lowLevel / 5 * 2) + 2));
            }
            else if (_stats.HPLevel >= 50 && _stats.HPLevel < 70)
            {
                uint lowLevel = _stats.HPLevel - 50;
                if (lowLevel % 2 != 0)
                {
                    requirements.Add(new(ResourceType.Gen1, lowLevel / 2 + 2));
                    requirements.Add(new(ResourceType.Gen4, lowLevel / 2 + 2));
                }
            }
            else if (_stats.HPLevel > 70)
            {
                uint lowLevel = _stats.HPLevel - 70;
                if (lowLevel % 2 != 0)
                {
                    requirements.Add(new(ResourceType.Gen2, (uint)Mathf.Min(6, lowLevel / 2 + 3)));
                    requirements.Add(new(ResourceType.Gen4, (uint)Mathf.Min(10, lowLevel / 2 + 3)));
                }
            }
            return requirements;
        }

        public static float GetDPS()
        {
            return 16 + _stats.DPSLevel * 0.05f;
        }
        public static float GetMS()
        {
            float baseValue = 16;

            if (_stats.DPSLevel <= 20) return baseValue + _stats.DPSLevel;
            else
            {
                uint extraLevels = _stats.DPSLevel - 20;
                return baseValue + 20 + extraLevels * 0.5f;
            }
        }
        public static float GetCapacity()
        {
            float baseValue = 4;

            if (_stats.CapacityLevel <= 25) return baseValue + _stats.CapacityLevel * 0.075f;
            else if (_stats.CapacityLevel > 25 && _stats.CapacityLevel <= 50)
            {
                uint extraLevels = _stats.CapacityLevel - 25;
                return baseValue = 25 * 0.075f + (extraLevels) * 0.055f;
            }
            else
            {
                uint extraLevels = _stats.CapacityLevel - 50;
                return baseValue = 25 * (0.075f + 0.055f) + (extraLevels) * 0.035f;
            }
        }
        public static float GetRadius()
        {
            float baseValue = 15.5f;

            if (_stats.RadiusLevel <= 20) return baseValue + _stats.RadiusLevel * 0.5f;
            else
            {
                uint extraLevels = _stats.RadiusLevel - 20;
                return baseValue + 20 * 0.5f + (extraLevels / 2) * 0.2f + ((extraLevels + 1) / 2) * 0.3f;
            }
        }
        public static float GetTentacle()
        {
            return 1 + _stats.TentacleLevel;
        }
        public static float GetHP()
        {
            float baseValue = 22;
            return baseValue + (_stats.HPLevel / 2) * 0.2f + ((_stats.HPLevel + 1) / 2) * 0.3f;
        }
    }
}