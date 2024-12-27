using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;

public class EnemyUI : MonoBehaviour
{
    private Enemy _manager;
    private Tweener _healthBarTween;

    [SerializeField] private Transform _billboardCanvas;
    [SerializeField] private Image _healthBar;
    [SerializeField] private TextMeshProUGUI _healthText;

    private UniTaskVoid _textCounting;
    private CancellationTokenSource _tokenSource = new();

    [Button]
    public void GetText()
    {
        _healthText = _healthBar.transform.parent.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Awake()
    {
        _manager = GetComponent<Enemy>();
    }

    private void Start()
    {
        Initialization();
    }

    public void Initialization()
    {
        _billboardCanvas.gameObject.SetActive(false);
        _healthBar.fillAmount = 1;
    }

    public void MonoUpdate()
    {
        Vector3 direction = Camera.main.transform.forward;
        Quaternion rotation = Quaternion.LookRotation(direction);

        _billboardCanvas.transform.rotation = rotation;
    }

    public void UpdateHealthBar(bool start)
    {
        if (start)
        {
            float time = _manager.Stats.CurrentHealth / Game.Data.PlayerStats.Attributes[AttributeType.DPS];
            _healthBarTween = _healthBar.DOFillAmount(0, time).SetEase(Ease.Linear).OnComplete(_manager.Movement.MoveTowardPlayer);
            if (!_billboardCanvas.gameObject.activeSelf) _billboardCanvas.gameObject.SetActive(true);

            _tokenSource = new();
            TextCounting(_healthBar.fillAmount * 100, 0, time).Forget();
        }
        else
        {
            float remainFillAmount = _healthBar.fillAmount;
            _manager.Stats.CurrentHealth = Mathf.FloorToInt(_manager.Stats.Stats.HP * remainFillAmount);

            if (_healthBarTween.IsNull()) return;
            _healthBarTween.Kill();
            _healthBarTween = null;

            float recoveryTime = 2 * (1 - remainFillAmount);

            _healthBarTween = _healthBar.DOFillAmount(1, recoveryTime).SetEase(Ease.Linear).OnUpdate(() =>
            {
                _manager.Stats.CurrentHealth = Mathf.FloorToInt(_manager.Stats.Stats.HP * _healthBar.fillAmount);
            }).OnComplete(() =>
            {
                _billboardCanvas.gameObject.SetActive(false);

                _healthBarTween.Kill();
                _healthBarTween = null;
            });

            if (!_tokenSource.IsNull())
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
                _tokenSource = new();

                TextCounting(_healthBar.fillAmount * 100, 100, recoveryTime).Forget();
            }
        }
    }

    private async UniTaskVoid TextCounting(float start, float end, float time)
    {
        float elapsedTime = 0;
        int currentValue = Mathf.RoundToInt(start);

        while (elapsedTime < time)
        {
            if (_tokenSource.Token.IsCancellationRequested) break;

            currentValue = Mathf.RoundToInt(Mathf.Lerp(start, end, elapsedTime / time));
            _healthText.text = $"{currentValue}%";
            elapsedTime += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, _tokenSource.Token);
        }

        if (!_tokenSource.Token.IsCancellationRequested)
        {
            _healthText.text = $"{end}%";
        }
    }
}