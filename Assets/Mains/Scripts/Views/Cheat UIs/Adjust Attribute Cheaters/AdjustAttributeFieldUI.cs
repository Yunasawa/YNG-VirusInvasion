using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using YNL.Bases;

public class AdjustAttributeFieldUI : MonoBehaviour
{
    public AttributeType Type;
    public TMP_InputField InputField;

    private void Awake()
    {
        InputField.onValueChanged.AddListener(OnValueChanged);
    }

    public void UpdateUI()
    {
        InputField.text = Game.Data.PlayerStats.Levels[Type].ToString();
    }

    private void OnValueChanged(string input)
    {
        input = Regex.Replace(input, @"[^\d]", "");

        Game.Data.PlayerStats.Levels[Type] = uint.Parse(input);

        Player.OnUpgradeAttribute?.Invoke(Type);
    }
}
