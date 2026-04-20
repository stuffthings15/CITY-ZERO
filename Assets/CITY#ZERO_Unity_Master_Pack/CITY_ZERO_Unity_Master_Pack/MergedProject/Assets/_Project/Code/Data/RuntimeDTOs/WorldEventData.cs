using System;

namespace CityZero.Data.RuntimeDTOs
{
    [Serializable]
    public sealed class WorldEventData
    {
        public string id;
        public string displayName;
        public string eventType;
        public TriggerConditionsData triggerConditions;
        public int durationSeconds;
        public string[] participants;
        public string rewardProfile;
        public float heatImpact;
        public bool mapReveal;
        public string[] followupHooks;
    }

    [Serializable]
    public sealed class TriggerConditionsData
    {
        public int[] timeWindow;
        public string[] districts;
        public string[] weatherExcludes;
        public int minStoryAct;
    }
}
