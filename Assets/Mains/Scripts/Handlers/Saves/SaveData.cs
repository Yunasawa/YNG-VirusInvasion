using System;
using UnityEngine;
using YNL.Bases;

[System.Serializable]
public class SaveData
{
    public TutorialToggle TutorialToggle = new();

    public PlayerStats PlayerStats = Game.Data.PlayerStats;
    public CapacityStats CapacityStats = Game.Data.CapacityStats;
    public RuntimeConstructStats RuntimeConstructStats = Game.Data.RuntimeStats.ConstructStats;

    public SerializeQuestStats SerializeQuestStats = new();

    public SerializableVector3 CurrentPosition = new(Player.Transform.position);
    public StageType CurrentStage = Player.Stage.CurrentStage;

    public SaveData()
    {
        Set();
    }

    public SaveData Set()
    {
        SerializeQuestStats.SerializeData();
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
}