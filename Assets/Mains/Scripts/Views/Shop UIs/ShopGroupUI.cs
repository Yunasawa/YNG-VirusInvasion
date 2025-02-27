using UnityEngine;

public class ShopGroupUI : MonoBehaviour
{
    [SerializeField] private ShopItemUI _freeItem;
    [SerializeField] private ShopItemUI _lowItem;
    [SerializeField] private ShopItemUI _highItem;

    public void Initialze(ShopGroup stats)
    {
        _freeItem.Initialize(stats.Resource, stats.FreeItem);
        _lowItem.Initialize(stats.Resource, stats.LowItem);
        _highItem.Initialize(stats.Resource, stats.HighItem);
    }
}