public static class Formula
{
    public static class Stats
    {
        public static ((ResourceType Type, ushort Value) Material, ushort Condition) GetDPSREquirements(ushort level)
        {
            if (level > 0 && level <= 50) return new(new(ResourceType.Food1, 23), 0);
            return new();
        }
    }
}