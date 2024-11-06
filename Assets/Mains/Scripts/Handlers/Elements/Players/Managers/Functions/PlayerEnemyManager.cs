using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class PlayerEnemyManager : MonoBehaviour
{
    public int MaxCatchingAmount = 1;
    public SerializableDictionary<Tentacle, bool> _tentacleSpaces = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            CharacterManager monster = other.GetComponent<CharacterManager>();
            if (monster.IsCaught) return;

            KeyValuePair<Tentacle, bool> pair = _tentacleSpaces.FirstOrDefault(i => !i.Value);
            if (!pair.Key.IsNull())
            {
                monster.Movement.Slowdown(true);
                monster.IsCaught = true;
                _tentacleSpaces[pair.Key] = true;
                pair.Key.SetTarget(monster.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Monster")
        {
            CharacterManager monster = other.GetComponent<CharacterManager>();
            if (!monster.IsCaught) return;

            KeyValuePair<Tentacle, bool> pair = new();

            foreach (var tentacle in _tentacleSpaces)
            {
                if (tentacle.Key.Target.IsNullOrDestroyed()) continue;
                if (tentacle.Key.Target.gameObject == monster.gameObject) pair = tentacle;
            }

            if (!pair.Key.IsNull())
            {
                monster.Movement.Slowdown(false);
                monster.IsCaught = false;
                _tentacleSpaces[pair.Key] = false;
                pair.Key.RemoveTarget();
            }
        }
    }
}

