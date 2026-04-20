using System;

namespace CityZero.Data.RuntimeDTOs
{
    [Serializable]
    public sealed class ShopData
    {
        public string id;
        public string displayName;
        public string shopType;
        public string district;
        public string[] inventoryPools;
        public UnlockRequirementsData unlockRequirements;
        public ShopHoursData hours;
        public ShopRiskData risk;
    }

    [Serializable]
    public sealed class UnlockRequirementsData
    {
        public FactionRepRequirementData[] minRepByFaction;
        public string[] storyFlags;
    }

    [Serializable]
    public sealed class FactionRepRequirementData
    {
        public string factionId;
        public int minimumRep;
    }

    [Serializable]
    public sealed class ShopHoursData
    {
        public int open;
        public int close;
    }

    [Serializable]
    public sealed class ShopRiskData
    {
        public float policeRaidChance;
        public float surveillance;
    }
}
