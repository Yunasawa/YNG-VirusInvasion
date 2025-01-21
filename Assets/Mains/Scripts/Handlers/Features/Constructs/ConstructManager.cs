using UnityEngine;
using UnityEngine.UI;

public class ConstructManager : MonoBehaviour
{
    public string Name;
    public ConstructType Type;
    public GameObject UI;
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button?.onClick.AddListener(OpenConstructWindow);
    }

    private void OpenConstructWindow()
    {
        View.OnOpenConstructWindow?.Invoke(Type, true, this);
    }
}