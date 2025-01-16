using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using YNL.Extensions.Methods;

public class SettingsToggleUI : MonoBehaviour
{
    [SerializeField] private Button _buttonUI;
    [SerializeField] private Image _frameUI;
    [SerializeField] private Image _handlerUI;

    public SettingsType Type;
    public bool IsToggled = false;

    private void Awake()
    {
        _buttonUI.onClick.AddListener(Toggle);
    }

    private void Start()
    {
        Toggle();
    }

    public void Toggle()
    {
        IsToggled = !IsToggled;

        _frameUI.color = IsToggled ? "#BA8A66".ToColor() : "#978A81".ToColor();
        _handlerUI.transform.DOLocalMoveX(IsToggled ? 50 : -50, 0.5f).SetEase(Ease.OutExpo);

        if (Type == SettingsType.Sound) Key.Config.EnableSound = IsToggled;
        if (Type == SettingsType.Music) Key.Config.EnableMusic = IsToggled;
        View.OnToggleSettings?.Invoke(Type);
    }
}