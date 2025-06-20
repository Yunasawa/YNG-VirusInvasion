using Sirenix.OdinInspector;
using YNL.Utilities.Addons;

namespace YNL.Bases
{
    [System.Serializable]
    public class ConstructStats
    {
        public SerializableDictionary<string, MarketStats> Markets = new();
        public SerializableDictionary<string, ExchangerStats> Exchangers = new();
        public SerializableDictionary<string, FarmStats> Farms = new();
    }

    [System.Serializable]
    public class MarketStats
    {
        public string Name;
        public MarketStatsNode[] Nodes = new MarketStatsNode[0];
        public MarketEvolutionNode[] Evolutions = new MarketEvolutionNode[0];
    }

    [System.Serializable]
    public class MarketStatsNode
    {
        public string Name;
        public string Key;
        public byte Evolution = 1;
        public string Description;
        public bool PercentValue;
        public float Value;
        public bool PercentCost;
        public ResourcesInfo BaseCost;
        public float StepCost;
    }

    [System.Serializable]
    public class MarketEvolutionNode
    {
        public int Evolution;
        public ResourcesInfo[] Costs = new ResourcesInfo[0];
    }

    [System.Serializable]
    public class ExchangerStats
    {
        public string Name;
        public ExchangerStatsNode[] Nodes = new ExchangerStatsNode[0];
    }

    [System.Serializable]
    public class ExchangerStatsNode
    {
        public ResourcesInfo From;
        public ResourcesInfo To;
    }

    [System.Serializable]
    public class FarmStats
    {
        public string Name;
        public ResourceType Resource;
        public FarmStatsNode[] Nodes = new FarmStatsNode[0];
    }

    [System.Serializable]
    public class FarmStatsNode
    {
        public string Name;
        public string Description;
        public float Value;
        public bool PercentCost;
        public ResourcesInfo BaseCost;
        public float StepCost;
    }
}