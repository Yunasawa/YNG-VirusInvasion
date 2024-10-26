using System.Collections.Generic;
using UnityEngine;
using YNL.Bases;
using BaseStats = YNL.Bases.Stats;

public static class Formula
{
    public static class Stats
    {
        private static BaseStats _stats => Game.Data.Stats;

        public static List<(ResourceType, uint)> GetDPSRequirement()
        {
            List<(ResourceType, uint)> requirements = new();
            requirements.Add(new(ResourceType.Protein, (uint)Mathf.CeilToInt(23 * Mathf.Pow(1 + 0.09f, _stats.DPSLevel))));

            if (_stats.DPSLevel >= 50 && _stats.DPSLevel < 70)
            {
                uint lowLevel = _stats.DPSLevel - 50;
                requirements.Add(new(ResourceType.Plasma, (uint)Mathf.Min(14, lowLevel / 2 + 1)));
            }
            else if (_stats.DPSLevel >= 70 && _stats.DPSLevel < 90)
            {
                uint lowLevel = _stats.DPSLevel - 70;
                requirements.Add(new(ResourceType.DNA1, (uint)Mathf.Min(6, lowLevel / 2 + 1)));
            }
            else if (_stats.DPSLevel >= 90 && _stats.DPSLevel < 100)
            {
                requirements.Add(new(ResourceType.DNA1, 8));
            }
            else if (_stats.DPSLevel > 100)
            {
                requirements.Add(new(ResourceType.DNA1, 10));
            }
            return requirements;
        }
        public static List<(ResourceType, uint)> GetMSRequirement()
        {
            List<(ResourceType, uint)> requirements = new();
            requirements.Add(new(ResourceType.Protein, (uint)Mathf.CeilToInt(23 * Mathf.Pow(1 + 0.2f, _stats.MSLevel))));

            if (_stats.MSLevel >= 35 && _stats.MSLevel < 40)
            {
                uint lowLevel = _stats.MSLevel - 35;
                requirements.Add(new(ResourceType.DNA1, (uint)Mathf.Min(4, lowLevel + 1)));
                requirements.Add(new(ResourceType.DNA2, (uint)Mathf.Min(4, lowLevel + 1)));
            }
            else if (_stats.MSLevel >= 40 && _stats.MSLevel < 50)
            {
                uint lowLevel = _stats.MSLevel - 40;
                requirements.Add(new(ResourceType.DNA1, (uint)Mathf.Min(8, lowLevel + 4)));
                requirements.Add(new(ResourceType.DNA4, (uint)Mathf.Min(10, lowLevel + 4)));
            }
            else if (_stats.MSLevel > 50)
            {
                uint lowLevel = _stats.MSLevel - 50;
                requirements.Add(new(ResourceType.DNA4, (uint)Mathf.Min(16, lowLevel + 8)));
            }
            return requirements;
        }
        public static List<(ResourceType, uint)> GetCapacityRequirement()
        {
            List<(ResourceType, uint)> requirements = new();
            requirements.Add(new(ResourceType.Protein, (uint)Mathf.CeilToInt(25 * Mathf.Pow(1 + 0.25f, _stats.CapacityLevel))));

            if (_stats.CapacityLevel >= 30 && _stats.CapacityLevel < 40)
            {
                uint lowLevel = _stats.CapacityLevel - 30;
                requirements.Add(new(ResourceType.DNA1, (uint)Mathf.Min(6, lowLevel + 5)));
                requirements.Add(new(ResourceType.DNA2, (uint)Mathf.Min(10, lowLevel + 8)));
            }
            else if (_stats.CapacityLevel >= 40 && _stats.CapacityLevel < 50)
            {
                uint lowLevel = _stats.CapacityLevel - 40;
                requirements.Add(new(ResourceType.DNA1, (uint)Mathf.Min(6, lowLevel + 2)));
                requirements.Add(new(ResourceType.DNA3, (uint)Mathf.Min(10, lowLevel + 8)));
            }
            else if (_stats.CapacityLevel > 50)
            {
                uint lowLevel = _stats.CapacityLevel - 50;
                requirements.Add(new(ResourceType.DNA3, (uint)Mathf.Min(6, lowLevel + 3)));
                requirements.Add(new(ResourceType.DNA4, (uint)Mathf.Min(10, lowLevel + 3)));
            }
            return requirements;
        }
        public static List<(ResourceType, uint)> GetRadiusRequirement()
        {
            List<(ResourceType, uint)> requirements = new();
            requirements.Add(new(ResourceType.Protein, (uint)Mathf.CeilToInt(500 * Mathf.Pow(1 + 0.03f, _stats.RadiusLevel))));

            if (_stats.RadiusLevel >= 30 && _stats.RadiusLevel < 50)
            {
                uint lowLevel = _stats.RadiusLevel - 30;
                requirements.Add(new(ResourceType.DNA1, (lowLevel / 5 * 2) + 2));
            }
            else if (_stats.RadiusLevel >= 50 && _stats.RadiusLevel < 70)
            {
                uint lowLevel = _stats.RadiusLevel - 50;
                if (lowLevel % 2 != 0)
                {
                    requirements.Add(new(ResourceType.DNA1, lowLevel / 2 + 2));
                    requirements.Add(new(ResourceType.DNA4, lowLevel / 2 + 2));
                }
            }
            else if (_stats.RadiusLevel > 70)
            {
                uint lowLevel = _stats.RadiusLevel - 70;
                if (lowLevel % 2 != 0)
                {
                    requirements.Add(new(ResourceType.DNA2, (uint)Mathf.Min(6, lowLevel / 2 + 3)));
                    requirements.Add(new(ResourceType.DNA4, (uint)Mathf.Min(10, lowLevel / 2 + 3)));
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
                    requirements.Add(new(ResourceType.Protein, 250));
                    break;
                case 2:
                    requirements.Add(new(ResourceType.Protein, 1000));
                    break;
                case 3:
                    requirements.Add(new(ResourceType.Protein, 2500));
                    requirements.Add(new(ResourceType.DNA1, 2));
                    break;
                case 4:
                    requirements.Add(new(ResourceType.Protein, 8000));
                    requirements.Add(new(ResourceType.DNA1, 3));
                    break;
                case 5:
                    requirements.Add(new(ResourceType.Protein, 22000));
                    requirements.Add(new(ResourceType.DNA1, 4));
                    break;
                case 6:
                    requirements.Add(new(ResourceType.Protein, 66000));
                    requirements.Add(new(ResourceType.DNA1, 4));
                    requirements.Add(new(ResourceType.DNA2, 2));
                    break;
                case 7:
                    requirements.Add(new(ResourceType.Protein, 198000));
                    requirements.Add(new(ResourceType.DNA1, 5));
                    requirements.Add(new(ResourceType.DNA4, 2));
                    break;
                case 8:
                    requirements.Add(new(ResourceType.Protein, 594000));
                    requirements.Add(new(ResourceType.DNA1, 6));
                    requirements.Add(new(ResourceType.DNA4, 2));
                    break;
                case 9:
                    requirements.Add(new(ResourceType.Protein, 1780000));
                    requirements.Add(new(ResourceType.DNA1, 4));
                    requirements.Add(new(ResourceType.DNA3, 7));
                    break;
                case 10:
                    requirements.Add(new(ResourceType.Protein, 5300000));
                    requirements.Add(new(ResourceType.DNA1, 5));
                    requirements.Add(new(ResourceType.DNA3, 8));
                    break;
            }

            return requirements;
        }
    }
}