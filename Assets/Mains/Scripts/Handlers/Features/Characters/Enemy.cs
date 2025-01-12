using Cysharp.Threading.Tasks;
using OWS.ObjectPooling;
using System;
using UnityEngine;
using static DG.Tweening.DOTweenCYInstruction;

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
        Stats.Model.SetActive(false);
        UI.UI.gameObject.SetActive(false);
        Stats.Particle.gameObject.SetActive(true);
        Stats.Particle.Play();

        WaitToKill().Forget();

        async UniTaskVoid WaitToKill()
        {
            await UniTask.WaitForSeconds(1);

            Stats.Model.SetActive(true);
            UI.UI.gameObject.SetActive(true);
            Stats.Particle.gameObject.SetActive(false);

            ReturnToPool();
            Pool.NotifyOnRespawn();
            this.gameObject.SetActive(false);

            Player.OnDefeatEnemy?.Invoke(Stats.ID);
        }
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