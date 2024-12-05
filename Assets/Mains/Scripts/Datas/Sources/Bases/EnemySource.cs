using UnityEngine;
using YNL.Extensions.Addons;

namespace YNL.Bases
{
    [System.Serializable]
    public class EnemySources
    {
        public MRange MaxEnemyInAnArea = new(8, 20);
        public Enemy Enemy;
        public int EnemyAmount => Mathf.RoundToInt(Random.Range(MaxEnemyInAnArea.Min, MaxEnemyInAnArea.Max));

        public EnemySources(Enemy enemy)
        {
            Enemy = enemy;
        }


    }
}