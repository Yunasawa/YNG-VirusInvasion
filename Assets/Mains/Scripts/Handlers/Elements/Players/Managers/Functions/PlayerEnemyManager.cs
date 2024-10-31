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

            //foreach (var tentacle in _tentacleSpaces)
            //{
            //    if (!tentacle.Value)
            //    {
            //        monster.transform.SetParent(tentacle.Key);
            //        monster.Movement.Enabled = false;
            //        monster.IsCaught = true;
            //        _tentacleSpaces[tentacle.Key] = true;
            //    }
            //}

            KeyValuePair<Tentacle, bool> pair = _tentacleSpaces.FirstOrDefault(i => !i.Value);
            if (!pair.Key.IsNull())
            {
                monster.Movement.Enabled = false;
                monster.IsCaught = true;
                _tentacleSpaces[pair.Key] = true;
                pair.Key.SetTarget(monster.transform);
            }
        }
    }
}

