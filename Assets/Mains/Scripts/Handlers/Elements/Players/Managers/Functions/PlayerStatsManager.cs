using UnityEngine;
using YNL.Bases;

public class PlayerStatsManager : MonoBehaviour
{
    private void Start()
    {
        for (byte i = 0; i < 6; i++) UpdateAttributeValues((AttributeType)i);
    }

    public void UpdateAttributeValues(AttributeType type)
    {
        switch (type)
        {
            case AttributeType.DPS: Game.Data.Stats.DPS = Formula.Stats.GetDPS(); break;
            case AttributeType.MS: Game.Data.Stats.MS = Formula.Stats.GetMS(); break;
            case AttributeType.Capacity: Game.Data.Stats.Capacity = Formula.Stats.GetCapacity(); break;
            case AttributeType.Radius: Game.Data.Stats.Radius = Formula.Stats.GetRadius(); break;
            case AttributeType.Tentacle: Game.Data.Stats.Tentacle = Formula.Stats.GetTentacle(); break;
            case AttributeType.HP: Game.Data.Stats.HP = Formula.Stats.GetHP(); break;
        }
    }
}