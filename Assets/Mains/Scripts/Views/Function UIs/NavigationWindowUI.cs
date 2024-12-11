using UnityEngine;
using UnityEngine.UI;
using YNL.Utilities.Addons;

public class NavigationWindowUI : MonoBehaviour
{
    [SerializeField] private SerializableDictionary<Button, GameObject> _buttonTabs = new();

    private void Awake()
    {
        foreach (var button in _buttonTabs)
        {
            button.Key.onClick.AddListener(() => button.Value.SetActive(true));
        }
    }
}