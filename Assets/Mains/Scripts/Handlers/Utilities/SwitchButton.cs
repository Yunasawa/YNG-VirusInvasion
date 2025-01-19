using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SwitchButton : MonoBehaviour, IPointerDownHandler
{
    [HideInInspector] public Button Button;
    public UnityEvent OnClick;

    private void Awake()
    {
        Button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}