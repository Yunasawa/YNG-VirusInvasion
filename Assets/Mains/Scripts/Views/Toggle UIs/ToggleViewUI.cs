using UnityEngine;

public class ToggleViewUI : MonoBehaviour
{
    protected TogglePanelUI _togglePanel;

    public void Initialize(TogglePanelUI togglePanel)
    {
        _togglePanel = togglePanel;
    }
}