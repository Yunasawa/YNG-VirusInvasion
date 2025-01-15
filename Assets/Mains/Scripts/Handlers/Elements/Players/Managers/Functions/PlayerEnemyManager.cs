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

#if false
    public override void OnColliderTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            Enemy monster = other.GetComponent<Enemy>();
            if (monster.IsCaught || Game.Data.CapacityStats.IsFull) return;
#if false
            foreach (var group in Tentacles)
            {
                CheckForOutboundEnemy(group);
                if (!group.Tentacle.IsEnabled) continue;
                if (group.Tentacle.HasTarget)
                {
                    if (group.Tentacle.Target != group.Enemy)
                    {
                        group.Tentacle.RemoveTarget();
                        group.Enemy = null;
                    }
                    else
                    {
                        if (!group.Enemy.IsCaught)
                        {
                            MDebug.Log("NONO");
                            group.Enemy.IsCaught = true;
                            group.Enemy.UI.UpdateHealthBar(true);
                        }
                    }

                    continue;
                }
                if (!group.Enemy.IsNull()) continue;

                group.Tentacle.SetTarget(monster);
                group.Enemy = monster;

                monster.IsCaught = true;
                monster.UI.UpdateHealthBar(true);

                break;
            }
#endif
        }
    }

    public override void OnColliderTriggerStay(Collider other)
    {
        OnColliderTriggerEnter(other);
    }

    public override void OnColliderTriggerExit(Collider other)
    {
        if (other.tag == "Monster")
        {
            Enemy monster = other.GetComponent<Enemy>();
            if (!monster.IsCaught) return;

#if false
            foreach (var group in Tentacles)
            {
                CheckForOutboundEnemy(group);
                if (!group.Tentacle.HasTarget || !group.Tentacle.IsEnabled) continue;

                monster.IsCaught = false;
                monster.UI.UpdateHealthBar(false);

                if (monster == group.Enemy)
                {
                    group.Tentacle.RemoveTarget();
                    group.Enemy = null;
                }

                break;
            }
#endif
        }
    }
#endif

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