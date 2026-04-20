using System;
using CityZero.Gameplay.Heat;
using CityZero.Gameplay.Missions;

namespace CityZero.Core.EventBus
{
    public static class GameEventBus
    {
        public static event Action<HeatLevelChangedEvent> HeatLevelChanged;
        public static event Action<CrimeCommittedEvent> CrimeCommitted;
        public static event Action<MissionStateChangedEvent> MissionStateChanged;

        public static void Raise(HeatLevelChangedEvent evt) => HeatLevelChanged?.Invoke(evt);
        public static void Raise(CrimeCommittedEvent evt) => CrimeCommitted?.Invoke(evt);
        public static void Raise(MissionStateChangedEvent evt) => MissionStateChanged?.Invoke(evt);
    }
}
