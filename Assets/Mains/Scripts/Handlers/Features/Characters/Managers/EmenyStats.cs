using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class EmenyStats : MonoBehaviour
{
    private Enemy _manager;

    public uint ID;

    public MonsterStats Stats;
    public uint CurrentHealth;

    private Tweener _healthBarTween;

    private void Awake()
    {
        _manager = GetComponent<Enemy>();
    }

    private void Start()
    {
        CurrentHealth = Stats.HP;
    }

    public void StartDamage(bool start)
    {
        if (start)
        {
            float time = CurrentHealth / Game.Data.Stats.DPS;
            _healthBarTween = _manager.UI.HealthBar.DOFillAmount(0, time).SetEase(Ease.Linear).OnComplete(_manager.Movement.MoveTowardPlayer);
        }
        else
        {
            float remainFillAmount = _manager.UI.HealthBar.fillAmount;
            CurrentHealth = (uint)Mathf.FloorToInt(Stats.HP * remainFillAmount);

            if (_healthBarTween.IsNull()) return;
            _healthBarTween.Kill();
            _healthBarTween = null;
        }
    }
    public void OnKilled()
    {
        _manager.OnEnemyKilled?.Invoke();
        _manager.OnEnemyKilled = null;
    }
}