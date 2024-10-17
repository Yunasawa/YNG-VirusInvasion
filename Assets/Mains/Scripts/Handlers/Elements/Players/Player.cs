using UnityEngine;

public static class Player
{
    public static Transform Transform => Manager.transform;
    public static PlayerManager Manager => PlayerManager.Instance;
    public static CharacterController Character => Manager.Character;
    public static Animator Animator => Manager.Animator;


    public static PlayerMovementManager Movement => Manager.Movement;
}
