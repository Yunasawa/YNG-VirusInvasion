using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerEnemyManager : ColliderTriggerListener
{
    private SerializableDictionary<string, int> _defeatedEnemies => Game.Data.RuntimeEnemy.EnemyStats.DefeatedEnemies;

    public List<TentaclePair> Tentacles = new();
    public List<Enemy> Enemies = new();

    [SerializeField] private SphereCollider _collider;
    [SerializeField] private Transform _radius;

    [SerializeField] private LayerMask _layerMask;
    private Collider[] _colliders = new Collider[10];

    private int _groupAmount;

    private List<Group<string, int>> _validEnemies = new();

    private void Awake()
    {
        Player.OnUpgradeAttribute += OnUpgradeAttribute;
        Player.OnEnterHomeBase += OnEnterHomeBase;
    }

    private void OnDestroy()
    {
        Player.OnUpgradeAttribute -= OnUpgradeAttribute;
        Player.OnEnterHomeBase -= OnEnterHomeBase;
    }

    private void Start()
    {
        OnUpgradeAttribute(AttributeType.Tentacle);
        OnUpgradeAttribute(AttributeType.Radius);
        OnUpgradeAttribute(AttributeType.MultiAttack);
    }


    private void Update()
    {
        int hitAmount = Physics.OverlapSphereNonAlloc(Player.Transform.position, Formula.Stats.EnemyRadius, _colliders, _layerMask);

        if (Game.Data.CapacityStats.IsFull) return;

        Enemies.Clear();
        for (int i = 0; i < hitAmount; i++) Enemies.Add(_colliders[i].GetComponent<Enemy>());

        foreach (var pair in Tentacles)
        {
            if (!pair.Tentacle.IsEnabled) break;
            if (pair.Tentacle.HasTarget)
            {
                if (pair.Tentacle.Target == pair.Enemy)
                {
                    if (!Enemies.Contains(pair.Enemy))
                    {
                        pair.Enemy.IsCaught = false;
                        pair.Enemy.UI.UpdateHealthBar(pair.Enemy.IsCaught);
                        pair.Tentacle.RemoveTarget();
                        pair.Enemy = null;
                    }
                    else if (Tentacles.Count(i => i.Enemy == pair.Enemy) > 1)
                    {
                        foreach (var enemy in Enemies)
                        {
                            if (!enemy.IsCaught)
                            {
                                enemy.IsCaught = true;
                                enemy.Movement.SetDamager(Player.Transform);
                                enemy.Stats.SetDamage(Game.Data.PlayerStats.Attributes[AttributeType.DPS]);

                                pair.Tentacle.SetTarget(enemy);
                                pair.Enemy = enemy;

                                break;
                            }
                        }

                        continue;
                    }
                }
                else
                {
                    if (pair.Tentacle.Target.IsCaught)
                    {
                        pair.Tentacle.Target.IsCaught = false;
                        pair.Tentacle.Target.UI.UpdateHealthBar(pair.Tentacle.Target.IsCaught);
                        pair.Tentacle.RemoveTarget();
                    }
                    if (!Enemies.Contains(pair.Enemy))
                    {
                        pair.Enemy.IsCaught = false;
                        pair.Enemy.UI.UpdateHealthBar(pair.Enemy.IsCaught);
                        pair.Tentacle.RemoveTarget();
                        pair.Enemy = null;
                    }
                    else
                    {
                        pair.Enemy.IsCaught = true;
                        pair.Enemy.Movement.SetDamager(Player.Transform);
                        pair.Enemy.Stats.SetDamage(Game.Data.PlayerStats.Attributes[AttributeType.DPS]);
                        pair.Enemy.UI.UpdateHealthBar(pair.Enemy.IsCaught);
                        pair.Tentacle.SetTarget(pair.Enemy);

                        continue;
                    }
                }
            }
            else
            {
                if (!pair.Enemy.IsNull())
                {
                    pair.Enemy.IsCaught = false;
                    pair.Enemy.UI.UpdateHealthBar(pair.Enemy.IsCaught);
                    pair.Tentacle.RemoveTarget();
                    pair.Enemy = null;
                }
            }
            if (!pair.Enemy.IsNull()) continue;

            foreach (var enemy in Enemies)
            {
                if (!enemy.IsEnable) continue;

                if (!enemy.IsCaught)
                {
                    enemy.IsCaught = true;
                    enemy.Movement.SetDamager(Player.Transform);
                    enemy.Stats.SetDamage(Game.Data.PlayerStats.Attributes[AttributeType.DPS]);
                    enemy.UI.UpdateHealthBar(enemy.IsCaught);

                    pair.Tentacle.SetTarget(enemy);
                    pair.Enemy = enemy;

                    break;
                }
                else if (Tentacles.Count(i => i.Enemy == enemy) < _groupAmount)
                {
                    pair.Tentacle.SetTarget(enemy);
                    pair.Enemy = enemy;

                    break;
                }
            }
        }

        foreach (var tentacle in Tentacles) tentacle.Enemy?.Stats.GetHit();
    }

    private void OnUpgradeAttribute(AttributeType type)
    {
        if (type == AttributeType.Tentacle)
        {
            for (int i = 0; i < Formula.Stats.GetTentacle(); i++)
            {
                Tentacles[i].Tentacle.IsEnabled = true;
            }
        }
        else if (type == AttributeType.Radius)
        {
            _collider.radius = Formula.Stats.EnemyRadius;
            _radius.localScale = Vector3.one * _collider.radius * 0.385f;
        }
        else if (type == AttributeType.MultiAttack)
        {
            _groupAmount = (int)Formula.Stats.GetGroupAttack();
        }
    }

    private void OnEnterHomeBase()
    {
        _validEnemies.Clear();
        Game.Data.RuntimeEnemy.EnemyStats.DefeatedEnemies.Clear();
    }

    public void DefeatEnemy(string name)
    {
        if (_defeatedEnemies.ContainsKey(name)) _defeatedEnemies[name]++;
        else _defeatedEnemies.Add(name, 1);
    }
    private void TakeEnemyResources(int capacity)
    {
        _validEnemies.Clear();
        int currentCapacity = 0;

        foreach (var enemy in _defeatedEnemies)
        {
            int enemyCapacity = Game.Data.EnemySources[enemy.Key].Capacity;
            int maxEnemyCount = Math.Min(enemy.Value, (capacity - currentCapacity) / enemyCapacity);

            if (maxEnemyCount > 0)
            {
                _validEnemies.Add(new(enemy.Key, maxEnemyCount));
                currentCapacity += enemyCapacity * maxEnemyCount;
            }

            if (currentCapacity >= capacity) break;
        }
    }
    public List<Group<string, int>> GetValidEnemies(int capacity)
    {
        TakeEnemyResources(capacity);

        foreach (var enemy in _validEnemies)
        {
            EnemySources source = Game.Data.EnemySources[enemy.E1];
            
            Player.Stats.OnRemoveEnemyDrops(source.Capacity, source.Drops, enemy.E2);

            _defeatedEnemies[enemy.E1] -= enemy.E2;
            if (_defeatedEnemies[enemy.E1] <= 0) _defeatedEnemies.Remove(enemy.E1);
        }

        return _validEnemies;
    }
}

[System.Serializable]
public class TentaclePair
{
    public Tentacle Tentacle;
    public Enemy Enemy;
}