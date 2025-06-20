using UnityEngine;
using YNL.Bases;

public static partial class Player
{
    public static bool IsMoving { get; set; } = false;


    public static Transform Transform => Manager.transform;
    public static PlayerManager Manager => PlayerManager.Instance;
    public static CharacterController Character => Manager.Character;
    public static Animator Animator => Manager.Animator;


    public static PlayerMovementManager Movement => Manager.Movement;
    public static PlayerStatsManager Stats => Manager.Stats;
    public static PlayerEnemyManager Enemy => Manager.Enemy;
    public static PlayerConstructionManager Construction => Manager.Construction;
    public static PlayerStageManager Stage => Manager.Stage;
}
