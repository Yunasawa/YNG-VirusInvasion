using System;
using System.Collections.Generic;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;

public class DeliveryWindowUI : MonoBehaviour
{
    [SerializeField] private Transform _resourceContainer;
    [SerializeField] private ResourceNodeUI _resourceNode;

    private Dictionary<ResourceType, ResourceNodeUI> _nodes = new();

    private void Awake()
    {
        _resourceContainer.transform.DestroyAllChildrenImmediate();

        _nodes.Clear();
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            ResourceNodeUI node = Instantiate(_resourceNode, _resourceContainer);
            node.gameObject.SetActive(false);
            _nodes.Add(type, node);
        }
    }

    public void UpdateResources(Dictionary<ResourceType, int> resources)
    {
        ClearResources();

        foreach (var resource in resources)
        {
            ResourceNodeUI node = _nodes[resource.Key];
            node.gameObject.SetActive(true);
            node.UpdateNode(resource.Value);
        }
    }

    public void ClearResources()
    {
        _resourceContainer.gameObject.SetActiveAllChildren(false);
    }
}