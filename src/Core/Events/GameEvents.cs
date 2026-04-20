using CityZero.Core;

namespace CityZero.Core.Events
{
    public readonly struct HeatChangedEvent : IGameEvent
    {
        public float NewHeat { get; }
        public int NewLevel { get; }
        public string Reason { get; }

        public HeatChangedEvent(float newHeat, int newLevel, string reason)
        {
            NewHeat = newHeat;
            NewLevel = newLevel;
            Reason = reason;
        }
    }

    public readonly struct PlayerDiedEvent : IGameEvent
    {
        public float LastHealthBeforeDeath { get; }
        public PlayerDiedEvent(float lastHealth) => LastHealthBeforeDeath = lastHealth;
    }

    public readonly struct PlayerDownedEvent : IGameEvent
    {
        public float BleedoutDuration { get; }
        public PlayerDownedEvent(float bleedoutDuration) => BleedoutDuration = bleedoutDuration;
    }

    public readonly struct MissionStateChangedEvent : IGameEvent
    {
        public string MissionId { get; }
        public MissionState OldState { get; }
        public MissionState NewState { get; }

        public MissionStateChangedEvent(string missionId, MissionState oldState, MissionState newState)
        {
            MissionId = missionId;
            OldState = oldState;
            NewState = newState;
        }
    }

    public readonly struct ObjectiveCompletedEvent : IGameEvent
    {
        public string MissionId { get; }
        public string ObjectiveId { get; }
        public bool IsOptional { get; }

        public ObjectiveCompletedEvent(string missionId, string objectiveId, bool isOptional)
        {
            MissionId = missionId;
            ObjectiveId = objectiveId;
            IsOptional = isOptional;
        }
    }

    public readonly struct FactionRepChangedEvent : IGameEvent
    {
        public string FactionId { get; }
        public float OldRep { get; }
        public float NewRep { get; }
        public string Reason { get; }

        public FactionRepChangedEvent(string factionId, float oldRep, float newRep, string reason)
        {
            FactionId = factionId;
            OldRep = oldRep;
            NewRep = newRep;
            Reason = reason;
        }
    }

    public readonly struct EconomyTransactionEvent : IGameEvent
    {
        public float Amount { get; }         // Positive = earned; negative = spent
        public float NewBalance { get; }
        public string Reason { get; }

        public EconomyTransactionEvent(float amount, float newBalance, string reason)
        {
            Amount = amount;
            NewBalance = newBalance;
            Reason = reason;
        }
    }

    public readonly struct WorldEventSpawnedEvent : IGameEvent
    {
        public string EventId { get; }
        public string District { get; }

        public WorldEventSpawnedEvent(string eventId, string district)
        {
            EventId = eventId;
            District = district;
        }
    }

    public readonly struct WeatherChangedEvent : IGameEvent
    {
        public WeatherState OldWeather { get; }
        public WeatherState NewWeather { get; }

        public WeatherChangedEvent(WeatherState oldWeather, WeatherState newWeather)
        {
            OldWeather = oldWeather;
            NewWeather = newWeather;
        }
    }

    public readonly struct VehicleDestroyedEvent : IGameEvent
    {
        public int VehicleInstanceId { get; }
        public string VehicleDefinitionId { get; }

        public VehicleDestroyedEvent(int instanceId, string definitionId)
        {
            VehicleInstanceId = instanceId;
            VehicleDefinitionId = definitionId;
        }
    }

    // Enums used by events
    public enum MissionState { Locked, Available, Briefed, Active, Completed, Failed }
    public enum WeatherState { Clear, Overcast, LightRain, HeavyRain, Fog, Storm }
}
