using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Extensions.Methods;

public class ExpPopperUI : MonoBehaviour
{
    private class ExpGroup
    {
        public TextMeshProUGUI Text;
        public RectTransform Rect;
        public bool Enable = false;

        public ExpGroup(TextMeshProUGUI text, RectTransform rect)
        {
            Text = text;
            Rect = rect;
        }
    }

    [SerializeField] private TextMeshProUGUI _poppingPrefab;
    [SerializeField] private int _maxPoppingAmount = 20;

    private List<ExpGroup> _poppingUIs = new();

    private void Awake()
    {
        Player.OnCollectEnemyExp += OnCollectEnemyExp;
    }

    private void OnDestroy()
    {
        Player.OnCollectEnemyExp -= OnCollectEnemyExp;
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

    private void OnCollectEnemyExp(int exp)
    {
        foreach (var popping in _poppingUIs)
        {
            if (popping.Enable) continue;

            AnimatePopping(popping).Forget();
            break;
        }

        async UniTaskVoid AnimatePopping(ExpGroup popping)
        {
            popping.Enable = true;

            Vector2 position = Formula.Value.GetRandom2DPosition(200, 250);

            popping.Rect.anchoredPosition = position.AddY(-50);
            popping.Rect.DOAnchorPos(position, 2).SetEase(Ease.Linear);

            popping.Text.text = $"+ {exp} EXP";
            popping.Text.DOColor(Color.white, 1).SetEase(Ease.Linear);

            await UniTask.WaitForSeconds(2);

            popping.Text.DOColor(Color.clear, 1).SetEase(Ease.Linear);
            popping.Enable = false;
        }
    }
}