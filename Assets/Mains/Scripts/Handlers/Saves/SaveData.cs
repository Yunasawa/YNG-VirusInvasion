using System;
using UnityEngine;
using YNL.Bases;

[System.Serializable]
public class SaveData
{
    public TutorialToggle TutorialToggle = new();

    public PlayerStats PlayerStats;
    public CapacityStats CapacityStats;
    public RuntimeConstructStats RuntimeConstructStats;

    public SerializeQuestStats SerializeQuestStats = new();

    public RuntimeEnemyStats RuntimeEnemyStats;

    public SerializableVector3 CurrentPosition;
    public StageType CurrentStage;

    public SaveData Set()
    {
        PlayerStats = Game.Data.PlayerStats;
        CapacityStats = Game.Data.CapacityStats;
        RuntimeConstructStats = Game.Data.RuntimeStats.ConstructStats;

        SerializeQuestStats.SerializeData();

        RuntimeEnemyStats = Game.Data.RuntimeEnemy.EnemyStats;

        CurrentPosition = new(Player.Transform.position);
        CurrentStage = Player.Stage.CurrentStage;

        return this;
    }
}

[Serializable]
public struct SerializableVector3
{
    public float X, Y, Z;

    public SerializableVector3(Vector3 vector)
    {
        X = vector.x;
        Y = vector.y;
        Z = vector.z;
    }

    public Vector3 ToVector3() => new Vector3(X, Y, Z);

    public override string ToString() => $"({X}, {Y}, {Z})";
}

[Serializable]
public class TutorialToggle
{
    public bool StartGameTutorial = Game.IsStartGameTutorialActivated;
    public bool ReturnToBaseTutorial = Game.IsReturnToBaseTutorialActivated;
    public bool UpgradeAttributeTutorial = Game.IsUpgradeAttributeTutorialActivated;
    public bool IsFocusOnMainQuest1FirstTime = Game.IsFocusOnMainQuest1FirstTime;
}