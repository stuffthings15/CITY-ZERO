namespace CityZero.Gameplay.Heat
{
    public readonly struct CrimeCommittedEvent
    {
        public readonly int Severity;
        public readonly int WitnessCount;
        public readonly float DistrictMultiplier;

        public CrimeCommittedEvent(int severity, int witnessCount, float districtMultiplier)
        {
            Severity = severity;
            WitnessCount = witnessCount;
            DistrictMultiplier = districtMultiplier;
        }
    }

    public readonly struct HeatLevelChangedEvent
    {
        public readonly int PreviousLevel;
        public readonly int CurrentLevel;

        public HeatLevelChangedEvent(int previousLevel, int currentLevel)
        {
            PreviousLevel = previousLevel;
            CurrentLevel = currentLevel;
        }
    }
}
