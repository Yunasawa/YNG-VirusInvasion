using UnityEngine;
using UnityEngine.UI;

public class PolicyWindowUI : MonoBehaviour
{
    [SerializeField] private GameObject _policyWindow;
    [SerializeField] private Button _policyButton;
    [SerializeField] private Button _acceptButton;

    private void Awake()
    {
        _policyButton.onClick.AddListener(OpenPolicy);
        _acceptButton.onClick.AddListener(CheckPolicy);   
    }

    private void Start()
    {
        if (Game.IsPolicyChecked) this.gameObject.SetActive(false);
    }

    private void OpenPolicy()
    {
        Application.OpenURL("https://doc-hosting.flycricket.io/viral-rush-privacy-policy/7f315ee4-b9ee-4e22-af6b-a3d38e205903/privacy");
    }

    private void CheckPolicy()
    {
        Game.IsPolicyChecked = true;
        this.gameObject.SetActive(false);
    }
}