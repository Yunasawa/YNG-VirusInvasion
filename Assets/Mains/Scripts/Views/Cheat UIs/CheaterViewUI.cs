using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using YNL.Extensions.Methods;

public abstract class CheaterViewUI : MonoBehaviour
{
    public Button Button;
    public RectTransform Rect;
    public float ExpandedHeight = 0;

    private bool _isExpanded = false;

    protected virtual void Awake()
    {
        Button.onClick.AddListener(OpenView);
    }

    public abstract void OnOpenView();

    private void OpenView()
    {
        _isExpanded = !_isExpanded;

        MDebug.Log("ASSD");

        Rect.DOSizeDelta(new Vector2(Rect.sizeDelta.x, _isExpanded ? ExpandedHeight : 75), 1).SetEase(Ease.OutExpo);

        OnOpenView();
    }
}