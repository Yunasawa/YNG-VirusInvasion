using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;

public class ResourceBarUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _button;

    public ResourceType Type;

    public void Initialize(Sprite icon)
    {
        _icon.sprite = icon;
    }
    public void UpdateResource(float amount) => _text.text = Mathf.FloorToInt(amount).RoundValue();
}