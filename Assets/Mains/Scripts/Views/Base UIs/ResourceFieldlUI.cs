using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

public class ResourceFieldUI : MonoBehaviour
{
    [SerializeField] private SerializableDictionary<ResourceType, ResourceNodeUI> _resourceNodes = new();

    private void Awake()
    {
        Player.OnChangeResources += UpdateResource;
    }

    private void OnDestroy()
    {
        Player.OnChangeResources -= UpdateResource;
    }

    private void Start()
    {
        UpdateResource();
    }

    private void UpdateResource()
    {
        foreach (var node in _resourceNodes)
        {
            node.Value.UpdateNode(Game.Data.PlayerStats.Resources[node.Key]);
        }
    }
}