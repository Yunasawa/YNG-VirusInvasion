using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

public class ResourceViewUI : ToggleViewUI
{
    [SerializeField] private SerializableDictionary<ResourceType, ResourceBarUI> _resourceBars = new();

    private void Awake()
    {
        View.OnOpenResourceView += OnOpenResourceView;
        Player.OnChangeResources += OnOpenResourceView;
    }

    private void OnDestroy()
    {
        View.OnOpenResourceView -= OnOpenResourceView;
        Player.OnChangeResources -= OnOpenResourceView;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        foreach (var pair in _resourceBars)
        {
            pair.Value.Initialize(Game.Data.Vault.ResourceIcons[pair.Key]);
        }
    }

    private void OnOpenResourceView()
    {
        foreach (var pair in _resourceBars)
        {
            pair.Value.UpdateResource(Game.Data.PlayerStats.Resources[pair.Key]);
        }
    }
}