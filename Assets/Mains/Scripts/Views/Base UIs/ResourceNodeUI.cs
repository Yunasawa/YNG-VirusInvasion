using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceNodeUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;

    public void UpdateNode(float amount)
    {
        _text.text = Mathf.FloorToInt(amount).RoundValue();
    }

    public void UpdateIcon(Sprite icon) => _icon.sprite = icon;
}