using Sirenix.OdinInspector;
using YNL.Utilities.Addons;

[System.Serializable]
public class RuntimeConstructStats
{
    public SerializableDictionary<string, RuntimeFarmStats> Farms = new();

    public void Reset()
    {
        foreach (var pair in Farms) pair.Value.Reset();
    }
}

[System.Serializable]
public class RuntimeFarmStats
{
    [System.Serializable]
    public class Attribute
    {
        public int Level;
        public float Step;

        public float Value => Step * Level;
        public float NextValue => Step * (Level + 1);

        public Attribute(int level, float step)
        {
            this.Level = level;
            this.Step = step;
        }
    }

    public float Current;
    public SerializableDictionary<string, Attribute> Attributes = new();

    public void Reset()
    {
        this.Current = 0;
        this.Attributes["Income"].Level = 0;
        this.Attributes["Capacity"].Level = 0;
    }
}