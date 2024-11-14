using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructFieldUI : MonoBehaviour
{
    public Button ConstructButton;
    public TextMeshProUGUI ConstructTitle;

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
        ConstructButton.gameObject.SetActive(false);
    }

    private void OnInteractWithConstruct(bool isEnter, ConstructType type)
    {
        if (isEnter)
        {
            ConstructButton.gameObject.SetActive(true);
            ConstructTitle.text = type.ToString();
        }
        else
        {
            ConstructButton.gameObject.SetActive(false);
        }
    }
}
