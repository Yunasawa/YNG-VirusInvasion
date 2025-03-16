using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class ShopWindowUI : MonoBehaviour
{
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _closeButton;

    [SerializeField] private GameObject _shopWindow;

    [SerializeField] private Transform _groupContainer;
    [SerializeField] private ShopGroupUI _shopGroupPrefab;

    [SerializeField] private SerializableDictionary<ResourceType, ResourceNodeUI> _resourceNodes = new();

    private void Awake()
    {
        _shopButton.onClick.AddListener(() => ShowWindow(true));
        _closeButton.onClick.AddListener(() => ShowWindow(false));

        Player.OnChangeResources += InitializeResourceFields;
    }

    private void OnDestroy()
    {
        Player.OnChangeResources -= InitializeResourceFields;
    }

    private void Start()
    {
        CreateShopGroups();
        InitializeResourceFields();
    }

    private void ShowWindow(bool show) => _shopWindow.SetActive(show);

    private void CreateShopGroups()
    {
        _groupContainer.DestroyAllChildren();

        foreach (var group in Game.Data.ShopStats.ShopGroups)
        {
            ShopGroupUI groupUI = Instantiate(_shopGroupPrefab, _groupContainer);
            groupUI.Initialze(group);
        }
    }

    private void InitializeResourceFields()
    {
        foreach (var node in _resourceNodes)
        {
            node.Value.UpdateIcon(node.Key);
            node.Value.UpdateValue(Game.Data.PlayerStats.Resources[node.Key]);
        }
    }
}