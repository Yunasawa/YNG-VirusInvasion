using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;

public class FarmBillboardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;

    [SerializeField] private Button _button;

    [SerializeField] private Image _fill;
    [SerializeField] private TextMeshProUGUI _amount;

    [SerializeField] private Image _icon;

    public Action OnCollectResource;

    public void Initialize(string name, ResourceType type)
    {
        _title.text = name;
        _icon.sprite = Game.Data.Vault.ResourceIcons[type];

        _button.onClick.AddListener(() => OnCollectResource?.Invoke());
    }

    public void UpdateUI(float current, int max)
    {
        _amount.text = $"{current.RoundToDigit(1)}/{max}";
        _fill.fillAmount = current / max;
    }
}