using System;

namespace CityZero.Data.RuntimeDTOs
{
    [Serializable]
    public sealed class MissionData
    {
        public string id;
        public string displayName;
        public string missionType;
        public string giver;
        public string district;
        public int recommendedTier;
        public MissionPrerequisitesData prerequisites;
        public MissionObjectiveData[] objectives;
        public MissionRewardsData rewards;
        public string[] failureConditions;
        public MissionRuntimeModifiersData runtimeModifiers;
    }

    [Serializable]
    public sealed class MissionPrerequisitesData
    {
        public string[] storyFlags;
        public FactionRepRequirementData[] minRep;
    }

    [Serializable]
    public sealed class MissionObjectiveData
    {
        public string type;
        public string target;
        public bool failOnAlert;
        public int timeLimit;
        public int requiredHeatBelow;
        public int timeout;
    }

    [Serializable]
    public sealed class MissionRewardsData
    {
        public int cash;
        public FactionRepRewardData[] rep;
        public string[] unlockFlags;
    }

    [Serializable]
    public sealed class FactionRepRewardData
    {
        public string factionId;
        public int amount;
    }

    [Serializable]
    public sealed class MissionRuntimeModifiersData
    {
        public bool spawnExtraPatrols;
        public float civilianDensityMultiplier;
        public float trafficMultiplier;
    }
}
