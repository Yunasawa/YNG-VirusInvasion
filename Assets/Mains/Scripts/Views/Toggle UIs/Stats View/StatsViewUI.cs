using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsViewUI : MonoBehaviour
{
    private RectTransform _rectTransform;

    [SerializeField] private Image _arrowUI;
    [SerializeField] private Button _viewButtonUI;
    [SerializeField] private float _expandedHeight;

    [SerializeField] private List<StatsBarUI> _statsBarUIs = new();

    private bool _isExpanded = false;

    private void Awake()
    {
        _rectTransform = this.GetComponent<RectTransform>();
        _viewButtonUI.onClick.AddListener(ExpandView);
    }

    public void ExpandView()
    {
        _isExpanded = !_isExpanded;

        float height = _isExpanded ? _expandedHeight : 100;
        _rectTransform.DOSizeDelta(new Vector2(_rectTransform.sizeDelta.x, height), 0.5f).SetEase(Ease.OutExpo);
        _arrowUI.transform.localRotation = Quaternion.Euler(0, 0, _isExpanded ? 0 : 180);

        if (_isExpanded)
        {
            foreach (var bar in _statsBarUIs) bar.UpdateContent();
        }
    }
}