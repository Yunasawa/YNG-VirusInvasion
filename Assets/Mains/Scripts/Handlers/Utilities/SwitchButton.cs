using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SwitchButton : MonoBehaviour, IPointerDownHandler
{
    [HideInInspector] public Image Graphic;
    public UnityEvent OnClick;

    private void Awake()
    {
        Graphic = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}