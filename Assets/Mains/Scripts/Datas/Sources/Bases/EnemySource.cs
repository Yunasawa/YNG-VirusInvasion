using UnityEngine;
using YNL.Extensions.Addons;

namespace YNL.Bases
{
    [System.Serializable]
    public class EnemySources
    {
        public MRange MaxEnemyInAnArea = new(8, 15);
        public uint RespawnTime = 10;
        public Enemy Enemy;

        public int EnemyAmount => Mathf.RoundToInt(Random.Range(MaxEnemyInAnArea.Min, MaxEnemyInAnArea.Max));
    }
}