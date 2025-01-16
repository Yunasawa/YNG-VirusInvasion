using TMPro;
using UnityEngine;
using YNL.Bases;

public class StatsBarUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textUI;
    [SerializeField] private string _content;
    [SerializeField] private AttributeType _type;

    public void UpdateContent()
    {
        float value = Game.Data.PlayerStats.Attributes[_type];
        _textUI.text = _content.Replace("@", $"<color=#7E4122>{value}</color>");
    }
}