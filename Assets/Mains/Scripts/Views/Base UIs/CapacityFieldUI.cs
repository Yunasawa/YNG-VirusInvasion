using Sirenix.OdinInspector;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Utilities.Addons;

public class CapacityFieldUI : MonoBehaviour
{
    [Title("Capacity Bar")]
    [SerializeField] private Image _capacityBar;
    [SerializeField] private TextMeshProUGUI _capacityText;

    [Title("Resource Bar")]
    [SerializeField] private SerializableDictionary<ResourceType, ResourceNodeUI> _collectedNodes = new();

    private void Awake()
    {
        Player.OnChangeCapacity += UpdateCapacity;
    }

    private void OnDestroy()
    {
        Player.OnChangeCapacity -= UpdateCapacity;
    }

    private void Start()
    {
        UpdateCapacity();
    }

    private void UpdateCapacity()
    {
        if (_capacityBar != null)
        {
            float fillAmount = (float)Game.Data.CapacityStats.CurrentCapacity / Mathf.FloorToInt(Game.Data.PlayerStats.Capacity);
            _capacityBar.fillAmount = fillAmount;
            _capacityText.text = $"Capacity: {Game.Data.CapacityStats.CurrentCapacity}/{Mathf.FloorToInt(Game.Data.PlayerStats.Capacity)}";
        }

        foreach (var pair in _collectedNodes)
        {
            if (Game.Data.CapacityStats.Resources.TryGetValue(pair.Key, out uint resourceAmount))
            {
                pair.Value.gameObject.SetActive(resourceAmount > 0);
                pair.Value.UpdateNode(resourceAmount);
            }
            else pair.Value.gameObject.SetActive(false);
        }
    }
}