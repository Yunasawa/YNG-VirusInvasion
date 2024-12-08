using UnityEngine;
using YNL.Extensions.Methods;

public class PlayerStageManager : ColliderTriggerListener
{
    public StageType CurrentStage;

    public override void OnColliderTriggerExit(Collider other)
    {
        if (other.tag == "Door")
        {
            Door door = other.GetComponent<Door>();
            if (door.IsNull()) return;

            StageType previousStage = CurrentStage;

            CurrentStage = CurrentStage == door.Stage1 ? door.Stage2 : door.Stage1;

            Player.OnEnterStage?.Invoke(previousStage, CurrentStage);
        }
    }
}