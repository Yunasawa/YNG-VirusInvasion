using YNL.Extensions.Addons;

namespace YNL.Bases
{
    [System.Serializable]
    public class EnemySources
    {
        public MRange MaxEnemyInAnArea = new(8, 20);
        public Enemy Enemy;
    }
}