using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using YNL.Extensions.Methods;

public class ShopWindowUI : MonoBehaviour
{
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _closeButton;

    [SerializeField] private GameObject _shopWindow;

    [SerializeField] private Transform _groupContainer;
    [SerializeField] private ShopGroupUI _shopGroupPrefab;

    private void Awake()
    {
        _shopButton.onClick.AddListener(() => ShowWindow(true));
        _closeButton.onClick.AddListener(() => ShowWindow(false));       
    }

    private void Start()
    {
        CreateShopGroups();
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
}