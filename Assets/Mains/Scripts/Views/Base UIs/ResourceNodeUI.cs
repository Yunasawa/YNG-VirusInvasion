using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;

public class ResourceNodeUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;

    public void UpdateValue(float amount)
    {
        _text.text = Mathf.FloorToInt(amount).RoundValue();
    }

    public void UpdateIcon(ResourceType type) => _icon.sprite = Game.Data.Vault.ResourceIcons[type];
}