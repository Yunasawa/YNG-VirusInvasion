using UnityEngine;
using UnityEngine.UI;

public class TogglePanelUI : MonoBehaviour
{
    [SerializeField] private Button _resourceViewButton;
    [SerializeField] private Button _statsViewButton;
    [SerializeField] private Button _settingsViewButton;

    private bool _isResourceViewOpened = false;

    [SerializeField] private ResourceViewUI _resourceViewUI;

    private void Awake()
    {
        _resourceViewButton.onClick.AddListener(ToggleView);
    }

    public void ToggleView()
    {
        _isResourceViewOpened = !_isResourceViewOpened;
        _resourceViewUI.gameObject.SetActive(_isResourceViewOpened);

        if (_isResourceViewOpened) View.OnOpenResourceView?.Invoke();
    }
}