using System.Collections.Generic;
using UnityEngine;
using YNL.Bases;
using YNL.Extensions.Methods;

public class PlayerEnemyManager : ColliderTriggerListener
{
    [SerializeField] private Tentacle _tentaclePrefab;
    [SerializeField] private Transform _tentacleContainer;
    public List<Tentacle> Tentacles = new();
    public List<Enemy> Enemies = new();

    [SerializeField] private SphereCollider _collider;
    [SerializeField] private Transform _radius;

    private float _timer;

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

    public override void OnColliderTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            Enemy monster = other.GetComponent<Enemy>();
            if (monster.IsCaught || Game.Data.CapacityStats.IsFull) return;

            foreach (var tentacle in Tentacles)
            {
                if (tentacle.HasTarget || !tentacle.IsEnabled) continue;

                tentacle.SetTarget(monster);

                monster.IsCaught = true;
                monster.UI.UpdateHealthBar(true);

                if (!Enemies.Contains(monster)) Enemies.Add(monster);

                break;
            }
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

            foreach (var tentacle in Tentacles)
            {
                if (!tentacle.HasTarget || tentacle.Target != monster || !tentacle.IsEnabled) continue;

                tentacle.RemoveTarget();

                monster.IsCaught = false;
                monster.UI.UpdateHealthBar(false);

                Enemies.TryRemove(monster);

                break;
            }
        }
    }

    private void OnUpgradeAttribute(AttributeType type)
    {
        if (type == AttributeType.Tentacle)
        {
            for (int i = 0; i < Formula.Stats.GetTentacle(); i++)
            {
                Tentacles[i].IsEnabled = true;
            }
        }
        else if (type == AttributeType.Radius)
        {
            _collider.radius = Formula.Stats.GetRadius() / 5;
            _radius.localScale = Vector3.one * _collider.radius * 0.385f;
        }
    }
}
