using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceNodeUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;

    public void UpdateNode(uint amount)
    {
        _text.text = amount.RoundValue();
    }
}