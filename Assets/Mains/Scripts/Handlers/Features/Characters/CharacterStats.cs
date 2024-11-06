using DG.Tweening;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class CharacterStats : MonoBehaviour
{
    private CharacterManager _manager;

    public MonsterStats Stats;
    public uint CurrentHealth;

    private Tweener _healthBarTween;

    private void Awake()
    {
        _manager = GetComponent<CharacterManager>();
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
            _healthBarTween = _manager.UI.HealthBar.DOFillAmount(0, time).SetEase(Ease.Linear);
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
}

[System.Serializable]
public struct MonsterStats
{
    public float Exp;
    public uint Capacity;
    public uint HP;
    public uint MS;
    public uint MaxInstancePerArea;
    public float SpawningTime;
    public SerializableDictionary<ResourceType, uint> Drops;
    public float AttackDamage;
    public float ExperimentMultiplier;
}