using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Extensions.Methods;

public class ConstructFieldUI : MonoBehaviour
{
    public Button ConstructButton;
    public TextMeshProUGUI ConstructTitle;

    private ConstructType _currentConstructType;
    private ConstructManager _currentConstructManager;

    private void Awake()
    {
        Player.OnInteractWithConstruct += OnInteractWithConstruct;
    }

    private void OnDestroy()
    {
        Player.OnInteractWithConstruct -= OnInteractWithConstruct;
    }

    private void Start()
    {
        //ConstructButton.gameObject.SetActive(false);
        ConstructButton.onClick.AddListener(OnOpenConstructWindow);
    }

    private void OnInteractWithConstruct(bool isEnter, ConstructType type, ConstructManager manager)
    {
        if (type == ConstructType.Base) return;
        if (type == ConstructType.Quest) return;

        if (isEnter)
        {
            _currentConstructType = type;
            _currentConstructManager = manager;
            //ConstructButton.gameObject.SetActive(true);
            ConstructTitle.text = type.ToString();

            Construct.CurrentConstruct = manager.Name;
        }
        else
        {
            //ConstructButton.gameObject.SetActive(false);

            View.OnOpenConstructWindow?.Invoke(_currentConstructType, false, _currentConstructManager);
        }
    }

    private void OnOpenConstructWindow()
    {
        View.OnOpenConstructWindow?.Invoke(_currentConstructType, true, _currentConstructManager);
    }
}
