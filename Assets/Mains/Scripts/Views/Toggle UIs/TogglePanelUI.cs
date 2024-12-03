using UnityEngine;
using UnityEngine.UI;
using YNL.Extensions.Methods;

public class TogglePanelUI : MonoBehaviour
{
    [SerializeField] private Button _resourceViewButton;
    [SerializeField] private Button _statsViewButton;
    [SerializeField] private Button _settingsViewButton;

    private bool _isResourceViewOpened = false;
    private bool _isStatsViewOpened = false;
    private bool _isSettingsViewOpened = false;

    [SerializeField] private ResourceViewUI _resourceViewUI;
    [SerializeField] private StatsViewUI _statsViewUI;

    private void Awake()
    {
        _resourceViewButton.onClick.AddListener(() => ToggleView(0));
        _statsViewButton.onClick.AddListener(() => ToggleView(1));
        _settingsViewButton.onClick.AddListener(() => ToggleView(2));
    }

    private void Start()
    {
        _resourceViewUI.Initialize(this);
        _statsViewUI.Initialize(this);
    }

    public void ToggleView(byte type)
    {
        if (type == 0)
        {
            _isResourceViewOpened = !_isResourceViewOpened;
            _resourceViewUI.gameObject.SetActive(_isResourceViewOpened);

            if (_isResourceViewOpened) View.OnOpenResourceView?.Invoke();
        }
        else if (type == 1)
        {
            _isStatsViewOpened = !_isStatsViewOpened;
            _statsViewUI.gameObject.SetActive(_isStatsViewOpened);
        }
        else if (type == 2)
        {
            _isSettingsViewOpened = !_isSettingsViewOpened;
        }
    }
}