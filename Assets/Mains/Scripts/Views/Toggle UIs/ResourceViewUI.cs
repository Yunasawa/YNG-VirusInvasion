using UnityEngine;
using YNL.Bases;
using YNL.Utilities.Addons;

public class ResourceViewUI : ToggleViewUI
{
    [SerializeField] private SerializableDictionary<ResourceType, ResourceBarUI> _resourceBars = new();

    private void Awake()
    {
        View.OnOpenResourceView += OnOpenResourceView;
    }

    private void OnDestroy()
    {
        View.OnOpenResourceView -= OnOpenResourceView;
    }

    private void OnOpenResourceView()
    {
        foreach (var pair in _resourceBars)
        {
            pair.Value.UpdateResource(Game.Data.PlayerStats.Resources[pair.Key]);
        }
    }

#if false
    [Button]
    public void GetAll()
    {
        _resourceBars.Clear();

        List<ResourceBarUI> list = gameObject.transform.Cast<Transform>().Select(i => i.GetComponent<ResourceBarUI>()).ToList();

        for (int i = 0; i < list.Count; i++)
        {
            _resourceBars.Add((ResourceType)i, list[i]);
            list[i].Type = (ResourceType)i;
        }
    }
#endif
}