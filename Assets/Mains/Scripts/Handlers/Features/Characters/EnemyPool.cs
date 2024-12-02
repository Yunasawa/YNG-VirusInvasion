using Cysharp.Threading.Tasks;
using OWS.ObjectPooling;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class EnemyPool : MonoBehaviour
{
    public SerializableDictionary<Transform, Enemy> Enemies = new();
    public List<ObjectPool<Enemy>> EnemyPools = new();

    public Boundary Boundary;

    public uint Amount;

    private void Start()
    {
        EnemyPools.Clear();
        foreach (var enemy in Enemies) EnemyPools.Add(new ObjectPool<Enemy>(enemy.Value.gameObject, enemy.Key));

        SpawnEnemy();
    }

    private void Update()
    {
        for (int i = 0; i < EnemyPools[0].activeObjects.Count; i++) EnemyPools[0].activeObjects[i].MonoUpdate();

        if (EnemyPools[0].activeObjects.Count < Amount)
        {
            Vector3 position = Boundary.GetRandomPositionInRandomBoundary();//new Vector3(Random.Range(-20f, 20f), 0.5f, Random.Range(-20f, 20f));
            EnemyPools[0].PullGameObject(position, Quaternion.identity);
        }
    }

    public void SpawnEnemy()
    {
        for (int i = 0; i < Amount; i++)
        {
            Vector3 position = Boundary.GetRandomPositionInRandomBoundary();//new Vector3(Random.Range(-20f, 20f), 0.5f, Random.Range(-20f, 20f));
            EnemyPools[0].PullGameObject(position, Quaternion.identity);
        }
    }
}

[System.Serializable]
public struct Bound
{
    public Vector3 TopFrontLeft;
    public Vector3 BottomBackRight;
}
