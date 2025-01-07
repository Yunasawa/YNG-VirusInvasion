using OWS.ObjectPooling;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable<Enemy>
{
    [HideInInspector] public EnemyPool Pool;

    public EmenyStats Stats;
    public EnemyMovement Movement;
    public EnemyUI UI;

    public bool IsCaught = false;

    private Action<Enemy> returnToPool;
    public Action OnEnemyKilled;

    public void OnKilled()
    {
        ReturnToPool();
        Pool.NotifyOnRespawn();
        this.gameObject.SetActive(false);

        Player.OnDefeatEnemy?.Invoke(Stats.ID);
    }

    public void Initialize(Action<Enemy> returnAction)
    {
        this.returnToPool = returnAction;

        IsCaught = false;
        Stats.Initialization();
        UI.Initialization();
    }

    public void MonoUpdate()
    {
        Movement.MonoUpdate();
        UI.MonoUpdate();
    }

    public void ReturnToPool()
    {
        returnToPool?.Invoke(this);
    }
}