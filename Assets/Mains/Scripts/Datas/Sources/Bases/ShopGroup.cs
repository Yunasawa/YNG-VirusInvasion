using System.Collections.Generic;
using YNL.Bases;

[System.Serializable]
public class ShopStats
{
    public List<ShopGroup> ShopGroups = new();
}

[System.Serializable]
public class ShopGroup
{
    public ResourceType Resource;
    public ShopItem FreeItem = new();
    public ShopItem LowItem = new();
    public ShopItem HighItem = new();
}

[System.Serializable]
public class ShopItem
{
    public uint Amount;
    public uint Cost;
}