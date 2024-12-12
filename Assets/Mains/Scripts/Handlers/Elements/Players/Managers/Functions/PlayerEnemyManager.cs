using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemyManager : ColliderTriggerListener
{
    public int MaxCatchingAmount = 1;
    public List<Tentacle> Tentacles = new();

    public override void OnColliderTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            Enemy monster = other.GetComponent<Enemy>();
            if (monster.IsCaught || Game.Data.CapacityStats.IsFull) return;

            foreach (var tentacle in Tentacles)
            {
                if (tentacle.HasTarget) continue;

                tentacle.SetTarget(monster);

                monster.Movement.Slowdown(true);
                monster.IsCaught = true;
                monster.UI.UpdateHealthBar(true);

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
                if (!tentacle.HasTarget || tentacle.Target != monster) continue;

                tentacle.RemoveTarget();

                monster.Movement.Slowdown(false);
                monster.IsCaught = false;
                monster.UI.UpdateHealthBar(false);

                break;
            }
        }
    }

    [Button]
    public void AddTentacles(Transform parent)
    {
        Tentacles.Clear();
        foreach (Transform child in parent) Tentacles.Add(child.GetComponent<Tentacle>());
    }
}
