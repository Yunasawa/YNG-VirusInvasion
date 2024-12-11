using DG.Tweening;
using Sirenix.OdinInspector;
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
        }
        else
        {
            float remainFillAmount = _healthBar.fillAmount;
            _manager.Stats.CurrentHealth = (uint)Mathf.FloorToInt(_manager.Stats.Stats.HP * remainFillAmount);

            if (_healthBarTween.IsNull()) return;
            _healthBarTween.Kill();
            _healthBarTween = null;
        }
    }
}