using UnityEngine;
using UnityEngine.UI;
using YNL.Extensions.Methods;

public class CheaterPanelUI : MonoBehaviour
{
    public Button OpenButton;
    public Button CloseButton;

    public GameObject CheatView;

    public Button EvolutionButton;

    private void Awake()
    {
        OpenButton.onClick.AddListener(() => CheatView.gameObject.SetActive(true));
        CloseButton.onClick.AddListener(() => CheatView.gameObject.SetActive(false));

        EvolutionButton.onClick.AddListener(A);
    }

    private void A()
    {
        Cheat.OnEvolution?.Invoke();
    }
}