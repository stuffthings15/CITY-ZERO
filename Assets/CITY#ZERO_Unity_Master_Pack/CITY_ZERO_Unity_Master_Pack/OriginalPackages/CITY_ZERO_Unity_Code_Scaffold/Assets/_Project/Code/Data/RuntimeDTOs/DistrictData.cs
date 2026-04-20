using System;

namespace CityZero.Data.RuntimeDTOs
{
    [Serializable]
    public sealed class DistrictData
    {
        public string id;
        public string displayName;
        public string theme;
        public string description;
        public float heatMultiplier;
        public float civilianDensityDay;
        public float civilianDensityNight;
        public float trafficDensityDay;
        public float trafficDensityNight;
        public float wealthLevel;
        public float policePatrolIntensity;
        public string[] dominantFactions;
        public string[] environmentTags;
        public string ambientAudioProfile;
        public WeightedEventData[] worldEventWeights;
        public string[] shopPools;
        public SpawnRulesData spawnRules;
    }

    [Serializable]
    public sealed class WeightedEventData
    {
        public string id;
        public float weight;
    }

    [Serializable]
    public sealed class SpawnRulesData
    {
        public string[] civilianArchetypes;
        public string[] vehiclePools;
    }
}
