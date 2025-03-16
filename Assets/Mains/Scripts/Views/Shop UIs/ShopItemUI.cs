using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemCost;

    [SerializeField] private GameObject _coverer;

    [SerializeField] private Button _button;

    public bool IsFreeItem;

    private ShopItem _shopItem;
    private ResourceType _resourceType;

    bool _enoughResource => Game.Data.PlayerStats.Resources[ResourceType.Currency3] >= _shopItem.Cost;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);

        Player.OnChangeResources += UpdateUI;
    }

    private void OnDestroy()
    {
        Player.OnChangeResources -= UpdateUI;
    }

    public void Initialize(ResourceType type, ShopItem item)
    {
        _shopItem = item;
        _resourceType = type;

        _icon.sprite = Game.Data.Vault.ResourceIcons[_resourceType];

        _itemName.text = $"<size=55><color=#FFFF2C>{item.Amount}</color></size>\n{Key.Resource.GetName(_resourceType)}";
        _itemCost.text = item.Cost == 0 ? "Free" : $"<sprite name={ResourceType.Currency3}>{item.Cost}";

        _itemCost.color = _enoughResource ? Color.white : Color.red;
    }

    private void UpdateUI()
    {
        if (!IsFreeItem) _itemCost.color = _enoughResource ? Color.white : Color.red;
    }

    private void OnClick()
    {
        if (!_enoughResource) return;

        if (IsFreeItem)
        {
            _coverer.SetActive(true);
            _itemCost.color = Color.red;
        }

        Player.OnConsumeResources?.Invoke(ResourceType.Currency3, _shopItem.Cost);
        Game.Data.PlayerStats.AdjustResources(_resourceType, (int)_shopItem.Amount);
        Player.OnChangeResources?.Invoke();
    }
}