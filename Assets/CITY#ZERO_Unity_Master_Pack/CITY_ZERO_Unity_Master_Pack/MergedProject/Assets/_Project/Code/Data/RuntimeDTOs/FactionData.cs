using System;

namespace CityZero.Data.RuntimeDTOs
{
    [Serializable]
    public sealed class FactionData
    {
        public string id;
        public string displayName;
        public string archetype;
        public string description;
        public string[] primaryDistricts;
        public string[] enemyFactions;
        public string[] allyFactions;
        public ReputationRangesData reputationRanges;
        public CombatStyleData combatStyle;
        public EconomyHooksData economyHooks;
        public VisualProfileData visualProfile;
    }

    [Serializable]
    public sealed class ReputationRangesData
    {
        public int hostile;
        public int neutral;
        public int friendly;
        public int trusted;
    }

    [Serializable]
    public sealed class CombatStyleData
    {
        public string[] preferredWeapons;
        public float aggression;
        public float vehicleUse;
        public float morale;
        public float backupCallChance;
    }

    [Serializable]
    public sealed class EconomyHooksData
    {
        public int fenceDiscountAtRep;
        public int smugglingJobsUnlockAtRep;
        public int rareVehicleAccessAtRep;
    }

    [Serializable]
    public sealed class VisualProfileData
    {
        public string primaryColor;
        public string secondaryColor;
        public string emblem;
    }
}
