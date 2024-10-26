namespace YNL.Bases
{
    [System.Serializable]
    public class Stats
    {
        public uint Protein;
        public uint Plasma;
        public uint Antigen;
        public uint StemCell;
        public uint Enzyme;
        public uint DNA1;
        public uint DNA2;
        public uint DNA3;
        public uint DNA4;

        public uint DPSLevel;
        public uint MSLevel;
        public uint CapacityLevel;
        public uint RadiusLevel;
        public uint TentacleLevel;

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
}