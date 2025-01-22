using UnityEngine;
using UnityEngine.UI;

public class CheaterPanelUI : MonoBehaviour
{
    public Button OpenButton;
    public Button CloseButton;

    public GameObject CheatView;

    private void Awake()
    {
        OpenButton.onClick.AddListener(() => CheatView.gameObject.SetActive(true));
        CloseButton.onClick.AddListener(() => CheatView.gameObject.SetActive(false));
    }
}