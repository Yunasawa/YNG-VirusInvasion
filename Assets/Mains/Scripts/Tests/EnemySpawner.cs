using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Extensions.Methods;

public class EnemySpawner : MonoBehaviour
{
    public GameObject Enemy;
    public int Amount;

    private void Start()
    {
        
    }

    public void Spawn()
    {
        Destroy();
        for (int i = 0; i < Amount; i++) Instantiate(Enemy, new Vector3(Random.Range(-20f, 20f), 0.5f, Random.Range(-20f, 20f)), Quaternion.identity, this.transform);
    }

    public void Destroy()
    {
        this.transform.DestroyAllChildren();
    }
}