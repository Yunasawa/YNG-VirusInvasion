using System;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;

public class UpgradeAttributeTutorialUI : TutorialWindowUI
{
    [SerializeField] private Button _focusingPanel;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private GameObject _upgradingPanel;
    [SerializeField] private GameObject _upgradeWindow;

    private void Awake()
    {
        _focusingPanel.onClick.AddListener(OnFocusingPanelClicked);
        _upgradeButton.onClick.AddListener(OnFocusingPanelClicked);

        View.OnCloseUpgradeWindow += OnCloseUpgradeWindow;
        Player.OnUpgradeAttribute += OnUpgradeAttribute;
    }

    private void OnDestroy()
    {
        View.OnCloseUpgradeWindow -= OnCloseUpgradeWindow;
        Player.OnUpgradeAttribute -= OnUpgradeAttribute;
    }

    public override void ShowTutorial()
    {
        if (Game.IsUpgradeAttributeTutorialActivated) return;
        Game.IsUpgradeAttributeTutorialActivated = true;

        _focusingPanel.gameObject.SetActive(true);
    }

    private void OnFocusingPanelClicked()
    {
        _upgradeWindow.SetActive(true);
        _focusingPanel.gameObject.SetActive(false);
        _upgradingPanel.SetActive(true);
    }

    private void OnCloseUpgradeWindow()
    {
        _upgradingPanel.SetActive(false);
    }

    private void OnUpgradeAttribute(AttributeType type)
    {
        _upgradingPanel.SetActive(false);
    }
}