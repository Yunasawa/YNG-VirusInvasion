using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using YNL.Bases;
using YNL.Extensions.Methods;

public class AdjustResourceFieldUI : MonoBehaviour
{
    public ResourceType Type;
    public TMP_InputField InputField;
    public Image Icon;

    private void Awake()
    {
        InputField.onValueChanged.AddListener(OnSubmit);
    }

    private void Start()
    {
        Icon.sprite = Game.Data.Vault.ResourceIcons[Type];
    }

    public void UpdateUI()
    {
        InputField.text = Game.Data.PlayerStats.Resources[Type].ToString();
    }

    private void OnSubmit(string input)
    {
        input = Regex.Replace(input, @"[^\d]", "");

        if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
        {
            if (value > float.MaxValue)
            {
                value = float.MaxValue;
                InputField.text = value.ToString();
            }
        }

        Game.Data.PlayerStats.Resources[Type] = value;

        Player.OnChangeResources?.Invoke();
    }
}