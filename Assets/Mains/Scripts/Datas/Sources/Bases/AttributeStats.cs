using YNL.Bases;
using YNL.Utilities.Addons;

[System.Serializable]
public class AttributeStats
{
    public SerializableDictionary<AttributeType, UpgradeInfo> UpgradeInfos = new();
}

[System.Serializable]
public class UpgradeInfo
{
    public string Name;
    public string Description;
}