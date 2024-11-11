using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class PlayerEnemyManager : MonoBehaviour
{
    public int MaxCatchingAmount = 1;
    //public SerializableDictionary<Tentacle, Enemy> _tentacleSpaces = new();
    public List<Tentacle> Tentacles = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            Enemy monster = other.GetComponent<Enemy>();
            if (monster.IsCaught) return;

            foreach (var tentacle in Tentacles)
            {
                if (tentacle.HasTarget) continue;

                tentacle.SetTarget(monster);

                monster.Movement.Slowdown(true);
                monster.IsCaught = true;
                monster.Stats.StartDamage(true);

                break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
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
                monster.Stats.StartDamage(false);

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
