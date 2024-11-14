using UnityEngine;
using YNL.Patterns.Singletons;

public class PlayerManager : Singleton<PlayerManager>
{
    public CharacterController Character;
    public Animator Animator;

    public PlayerMovementManager Movement;
    public PlayerStatsManager Stats;
    public PlayerEnemyManager Enemy;
    public PlayerInteractingManager Interacting;
    public PlayerConstructionManager Construction;
}