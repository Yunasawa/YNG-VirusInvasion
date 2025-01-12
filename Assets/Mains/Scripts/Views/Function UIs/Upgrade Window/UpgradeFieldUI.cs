using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;

public class UpgradeFieldUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _cost;
    [SerializeField] private Button _upgradeButton;

    List<(ResourceType type, int amount)> _requirements = new();

    public AttributeType Type;

    private void Awake()
    {
        _upgradeButton.onClick.AddListener(UpgradeAttribute);

        Player.OnChangeResources += UpdateField;
    }

    private void OnDestroy()
    {
        Player.OnChangeResources -= UpdateField;
    }

    public void UpdateField()
    {
        uint level = Game.Data.PlayerStats.Levels[Type];
        _text.text = $"<size=45><color=#D9393E>{Game.Data.AttributeStats.UpgradeNames[Type].ToUpper()}</color></size>: Level <color=#0058AF>{level}</color> > <color=#0058AF>{level + 1}</color>\nIncrease <color=#A18B00>{Formula.Stats.GetAttributeValue(Type, level)}</color> > <color=#0C7B3E>{Formula.Stats.GetAttributeValue(Type, level + 1)}</color> {Type}";

        _requirements = Formula.Stats.GetAttributeRequirement(Type);
        _cost.text = "";

        if (Game.Data.Vault.UpgradeIcons.TryGetValue(Type, out Sprite icon))
        {
            _icon.sprite = icon;
        }

        bool enoughRequirement = true;

        foreach (var requirement in _requirements)
        {
            _cost.text += $"{requirement.amount}<sprite name={requirement.type}>";
            if (!_requirements.IsLast(requirement)) _cost.text += " ";

            if (Game.Data.PlayerStats.Resources[requirement.type] < requirement.amount) enoughRequirement = false;
        }

        if (Type == AttributeType.Tentacle && Game.Data.PlayerStats.Levels[AttributeType.Tentacle] >= 11)
        {
            _text.text = $"Max lvl <color=#0058AF>{level}</color>: <color=#A18B00>{Formula.Stats.GetAttributeValue(Type, level)}</color> {Type}";
            enoughRequirement = false;
        }

        if (enoughRequirement)
        {
            _upgradeButton.interactable = true;
            _cost.color = Color.white;
        }
        else
        {
            _upgradeButton.interactable = false;
            _cost.color = Color.red;
        }
    }

    private void UpgradeAttribute()
    {
        Game.Data.PlayerStats.UpgradeLevels(Type);
        Game.Data.PlayerStats.UpdateAttributes(Type);

        foreach (var requirement in _requirements) Player.OnConsumeResources?.Invoke(requirement.type, requirement.amount);

        Player.OnChangeResources?.Invoke();
        Player.OnUpgradeAttribute?.Invoke(Type);

        UpdateField();
    }
}