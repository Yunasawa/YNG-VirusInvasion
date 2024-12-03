using UnityEngine;
using UnityEngine.UI;

public class StatsViewUI : ToggleViewUI
{
    [SerializeField] private Button _closeButton;

    private void Awake()
    {
        _closeButton.onClick.AddListener(() => _togglePanel.ToggleView(1));
    }
}