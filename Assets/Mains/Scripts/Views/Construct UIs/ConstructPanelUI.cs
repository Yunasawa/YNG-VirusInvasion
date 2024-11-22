using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Utilities.Addons;

public class ConstructPanelUI : MonoBehaviour
{
    public GameObject Panel;
    public SerializableDictionary<ConstructType, ConstructWindowUI> Windows = new();
    public ConstructType CurrentWindow;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _panelTitle;

    private void Awake()
    {
        View.OnOpenConstructWindow += OnOpenConstructWindow;
    }

    private void OnDestroy()
    {
        View.OnOpenConstructWindow -= OnOpenConstructWindow;
    }

    private void Start()
    {
        foreach (var window in Windows) window.Value?.gameObject.SetActive(false);
        Panel.SetActive(false);

        _closeButton.onClick.AddListener(CloseCurrentWindow);
    }

    private void CloseCurrentWindow()
    {
        View.OnOpenConstructWindow?.Invoke(CurrentWindow, false, null);
    }

    public void OnOpenConstructWindow(ConstructType type, bool isOpen, ConstructManager manager)
    {
        if (Windows.ContainsKey(type))
        {
            Windows[type].gameObject.SetActive(isOpen);
            if (isOpen) Windows[type].OnOpenWindow();
            Panel.SetActive(isOpen);
        }
        if (isOpen)
        {
            CurrentWindow = type;
            _panelTitle.text = manager.Name;
        }
    }
}