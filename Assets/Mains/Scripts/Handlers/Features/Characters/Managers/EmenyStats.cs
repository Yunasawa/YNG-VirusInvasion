using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;

public class EmenyStats : MonoBehaviour
{
    public EnemySources Stats => Game.Data.EnemySources[ID];

    private Enemy _manager;

    public string ID;
    public int CurrentHealth;
    public GameObject Model;
    public ParticleSystem Particle;

    public float TimePassed;
    private float _timeToDeath;

    private void Awake()
    {
        _manager = GetComponent<Enemy>();
    }

    private void Start()
    {
        Initialization();

        Particle.gameObject.SetActive(false);
    }

    public void Initialization()
    {
        CurrentHealth = Stats.HP;
    }

    public void OnKilled()
    {
        _manager.OnEnemyKilled?.Invoke();
        _manager.OnEnemyKilled = null;
        Player.Enemy.DefeatEnemy(ID);
        Player.OnCollectEnemyDrops?.Invoke(Stats.Capacity, Stats.Drops);
        Player.OnCollectEnemyExp?.Invoke(Stats.Exp);
    }

    public void GetHit()
    {
        if (!_manager.IsCaught) return;

        if (!_manager.UI.UI.gameObject.activeSelf && _manager.IsEnable) _manager.UI.UI.gameObject.SetActive(true);

        float currentDamage = Game.Data.PlayerStats.Attributes[AttributeType.DPS] * 1;
        _timeToDeath = _manager.Stats.CurrentHealth / currentDamage;

        TimePassed += Time.deltaTime;

        if (TimePassed < _timeToDeath)
        {
            float percent = 1 - (TimePassed / _timeToDeath);
            _manager.UI.UpdateHealth(percent);
        }
        else
        {
            _manager.Movement.MoveTowardPlayer();
        }
    }

    [Button]
    public void Assign()
    {
        foreach (var child in transform.Cast<Transform>())
        {
            if (child.name.Contains("Enemy") || child.name.Contains("monster")) Model = child.gameObject;
            else if (child.name.Contains("BloodExplosionRound")) Particle = child.GetComponent<ParticleSystem>();
        }
    }
}