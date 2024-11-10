using Sirenix.OdinInspector;

namespace YNL.Bases
{
    [System.Serializable]
    public class PlayerStats
    {
        [Title("Resources")]
        public uint Protein;
        public uint Plasma;
        public uint Antigen;
        public uint StemCell;
        public uint Enzyme;
        public uint DNA1;
        public uint DNA2;
        public uint DNA3;
        public uint DNA4;

        [Title("Levels")]
        public uint DPSLevel = 1;
        public uint MSLevel = 1;
        public uint CapacityLevel = 1;
        public uint RadiusLevel = 1;
        public uint TentacleLevel = 1;
        public uint HPLevel = 1;

        [Title("Attributes")]
        public float DPS;
        public float MS;
        public float Capacity;
        public float Radius;
        public float Tentacle;
        public float HP;
    }

    public enum ResourceType
    {
        Protein, 
        Plasma, 
        Antigen, 
        StemCell, 
        Enzyme, 
        DNA1, 
        DNA2, 
        DNA3, 
        DNA4
    }

    public enum AttributeType : byte { DPS, MS, Capacity, Radius, Tentacle, HP }
}