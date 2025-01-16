using Sirenix.OdinInspector;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using YNL.Bases;

public class EmenyStats : MonoBehaviour
{
    public EnemySources Stats => Game.Data.EnemySources[ID];

    private Enemy _manager;

    public string ID;
    public int CurrentHealth;
    public GameObject Model;
    public ParticleSystem Particle;

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
        Player.OnCollectEnemyDrops?.Invoke(Stats.Capacity, Stats.Drops);
        Player.OnCollectEnemyExp?.Invoke(Stats.Exp);
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