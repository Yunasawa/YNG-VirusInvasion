using Sirenix.OdinInspector;
using YNL.Extensions.Methods;

namespace YNL.Bases
{
    [System.Serializable]
    public class ConstructStats
    {
        public MarketStats[] Markets = new MarketStats[0];
        public ExchangerStats[] Exchangers = new ExchangerStats[0];
    }

    [System.Serializable]
    public class MarketStats
    {
        public string Name;
        public MarketStatsNode[] Nodes = new MarketStatsNode[0];
    }

    [System.Serializable]
    public class MarketStatsNode
    {
        public string Name;
        public bool PercentValue;
        public float Value;
        public bool PercentCost;
        public ResourcesInfo BaseCost;
        public float StepCost;
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
}