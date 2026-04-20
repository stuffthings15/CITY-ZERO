namespace CityZero.Gameplay.Missions
{
    public readonly struct MissionStateChangedEvent
    {
        public readonly string MissionId;
        public readonly MissionState State;

        public MissionStateChangedEvent(string missionId, MissionState state)
        {
            MissionId = missionId;
            State = state;
        }
    }
}
