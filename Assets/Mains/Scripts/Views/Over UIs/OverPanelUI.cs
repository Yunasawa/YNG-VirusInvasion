using UnityEngine;
using UnityEngine.UI;

public class OverPanelUI : MonoBehaviour
{
    [SerializeField] private Button _panelButton;

    private void Awake()
    {
        _panelButton.onClick.AddListener(OnRespawn);

        Player.OnDeath += () => EnablePanel(true);
    }

    private void OnDestroy()
    {
        Player.OnDeath += () => EnablePanel(true);
    }

    private void Start()
    {
        EnablePanel(false);
    }

    private void EnablePanel(bool enabled = true) => this.gameObject.SetActive(enabled);
    private void OnRespawn()
    {
        EnablePanel(false);

        Player.OnRespawn?.Invoke();
    }
}