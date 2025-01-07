using System;
using UnityEngine;
using YNL.Bases;

[System.Serializable]
public class SaveData
{
    public TutorialToggle TutorialToggle = new();

    public PlayerStats PlayerStats = new();
    public CapacityStats CapacityStats = new();
    public RuntimeConstructStats RuntimeConstructStats = new();

    public SerializableVector3 CurrentPosition;
    public StageType CurrentStage;
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
    public bool StartGameTutorial = false;
    public bool ReturnToBaseTutorial = false;
    public bool UpgradeAttributeTutorial = false;
}