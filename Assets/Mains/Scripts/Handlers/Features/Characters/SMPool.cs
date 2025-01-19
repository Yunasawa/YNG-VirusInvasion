using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using YNL.Extensions.Methods;

public class SMPool : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;

    private EnemyPool _pool;

    private void Awake()
    {
        _pool = GetComponent<EnemyPool>();

        _pool.OnWaitToRespawn += OnWaitToRespawn;
    }

    private void OnDestroy()
    {
        _pool.OnWaitToRespawn -= OnWaitToRespawn;
    }

    private void OnWaitToRespawn(uint time)
    {
        WaitToRespawn(time - 1).Forget();
    }

    private async UniTask WaitToRespawn(uint time)
    {
        _text.gameObject.SetActive(true);

        while (time > 0)
        {
            string formattedTime = TimeSpan.FromSeconds(time).ToString(@"hh\:mm\:ss");
            _text.text = $"Next unit\n{formattedTime}";

            time--;

            await UniTask.WaitForSeconds(1);
        }

        _text.gameObject.SetActive(false);
    }
}