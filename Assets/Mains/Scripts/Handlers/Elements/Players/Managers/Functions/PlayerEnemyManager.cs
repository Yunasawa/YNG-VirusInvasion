using System.Collections.Generic;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;

public class PlayerEnemyManager : ColliderTriggerListener
{
    public List<TentaclePair> Tentacles = new();
    public List<Enemy> Enemies = new();

    [SerializeField] private SphereCollider _collider;
    [SerializeField] private Transform _radius;

    [SerializeField] private LayerMask _layerMask;
    private Collider[] _colliders = new Collider[10];

    private void Awake()
    {
        Player.OnUpgradeAttribute += OnUpgradeAttribute;
    }

    private void OnDestroy()
    {
        Player.OnUpgradeAttribute -= OnUpgradeAttribute;
    }

    private void Start()
    {
        OnUpgradeAttribute(AttributeType.Tentacle);
        OnUpgradeAttribute(AttributeType.Radius);
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
                if (!enemy.IsCaught && enemy.IsEnable)
                {
                    enemy.IsCaught = true;
                    enemy.UI.UpdateHealthBar(enemy.IsCaught);

                    pair.Tentacle.SetTarget(enemy);
                    pair.Enemy = enemy;

                    break;
                }
            }
        }

        foreach (var enemy in Enemies) enemy.Stats.GetHit();
    }


    private void CheckForOutboundEnemy(TentaclePair pair)
    {
        if (pair.Enemy.IsNull()) return;

        if (Vector3.Distance(pair.Enemy.transform.position, Player.Transform.position) > Formula.Stats.EnemyRadius * 1.25f)
        {
            MDebug.Log("HOHO");

            pair.Enemy.IsCaught = false;
            pair.Enemy.UI.UpdateHealthBar(false);
            pair.Enemy = null;

            pair.Tentacle.RemoveTarget();
        }
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
    }
}

[System.Serializable]
public class TentaclePair
{
    public Tentacle Tentacle;
    public Enemy Enemy;
}