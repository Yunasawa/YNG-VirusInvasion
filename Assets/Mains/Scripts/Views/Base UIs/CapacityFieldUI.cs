using Sirenix.OdinInspector;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class CapacityFieldUI : MonoBehaviour
{
    [Title("Capacity Bar")]
    [SerializeField] private Image _capacityBar;
    [SerializeField] private TextMeshProUGUI _capacityText;

    private (Color Normal, Color Full) _capacityColor = new("#01BE4D".ToColor(), "#BE3501".ToColor());

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
            float fillAmount = (float)Game.Data.CapacityStats.CurrentCapacity / Mathf.FloorToInt(Game.Data.PlayerStats.Attributes[AttributeType.Capacity]);
            _capacityBar.fillAmount = fillAmount;
            if (fillAmount >= 0.95f)
            {
                _capacityText.color = _capacityColor.Full;
                _capacityText.text = $"Capacity is full";
            }
            else
            {
                _capacityText.color = _capacityColor.Normal;
                _capacityText.text = $"Capacity: {Game.Data.CapacityStats.CurrentCapacity}/{Mathf.FloorToInt(Game.Data.PlayerStats.Attributes[AttributeType.Capacity])}";
            }
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