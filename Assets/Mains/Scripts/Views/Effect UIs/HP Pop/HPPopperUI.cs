using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Extensions.Methods;

public class HPPopperUI : MonoBehaviour
{
    private class HPGroup
    {
        public TextMeshProUGUI Text;
        public RectTransform Rect;
        public bool Enable = false;

        public HPGroup(TextMeshProUGUI text, RectTransform rect)
        {
            Text = text;
            Rect = rect;
        }
    }

    [SerializeField] private TextMeshProUGUI _poppingPrefab;
    [SerializeField] private int _maxPoppingAmount = 20;

    private List<HPGroup> _poppingUIs = new();

    private (Color Red, Color Green) _textColor;

    private void Awake()
    {
        _textColor = new("#FF2A2A".ToColor(), "#2AFF74".ToColor());

        Player.OnChangeHP += OnChangeHP;
    }

    private void OnDestroy()
    {
        Player.OnChangeHP -= OnChangeHP;
    }

    private void Start()
    {
        CreatePoppings();
    }

    private void CreatePoppings()
    {
        this.transform.DestroyAllChildren();
        _poppingUIs.Clear();
        for (int i = 0; i < _maxPoppingAmount; i++)
        {
            TextMeshProUGUI popping = Instantiate(_poppingPrefab, this.transform);
            _poppingUIs.Add(new(popping, popping.GetComponent<RectTransform>()));
            popping.color = Color.clear;
        }
    }

    private void OnChangeHP(bool isDamaged, int amount)
    {
        if (amount <= 0) return;

        foreach (var popping in _poppingUIs)
        {
            if (popping.Enable) continue;

            AnimatePopping(popping).Forget();
            break;
        }

        async UniTaskVoid AnimatePopping(HPGroup popping)
        {
            popping.Enable = true;

            Vector2 position = Formula.Value.GetRandom2DPosition(100, 50);

            popping.Rect.anchoredPosition = position.AddY(-50);
            popping.Rect.DOAnchorPos(position, 2).SetEase(Ease.Linear);

            string symbol = isDamaged ? "-" : "+";
            popping.Text.text = $"{symbol} {amount} HP";
            popping.Text.color = Color.clear;
            popping.Text.DOColor(isDamaged ? _textColor.Red : _textColor.Green, 0.5f).SetEase(Ease.Linear);

            await UniTask.WaitForSeconds(2);

            popping.Text.DOColor(Color.clear, 0.5f).SetEase(Ease.Linear);
            popping.Enable = false;
        }
    }
}