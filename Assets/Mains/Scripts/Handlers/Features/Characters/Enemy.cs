using OWS.ObjectPooling;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable<Enemy>
{
    public EmenyStats Stats;
    public EnemyMovement Movement;
    public EnemyUI UI;

    public bool IsCaught = false;

    private Action<Enemy> returnToPool;
    public Action OnEnemyKilled;

    private void OnDisable()
    {
        ReturnToPool();
    }

    public void Initialize(Action<Enemy> returnAction)
    {
        this.returnToPool = returnAction;
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