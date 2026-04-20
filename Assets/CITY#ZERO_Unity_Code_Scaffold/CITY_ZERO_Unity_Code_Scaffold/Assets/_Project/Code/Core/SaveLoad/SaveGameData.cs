using System;
using System.Collections.Generic;

namespace CityZero.Core.SaveLoad
{
    [Serializable]
    public sealed class SaveGameData
    {
        public string profileId = "default";
        public string currentDistrictId;
        public float worldTimeHours;
        public int cash;
        public List<string> unlockedFlags = new();
        public List<string> completedMissionIds = new();
        public List<string> ownedVehicleIds = new();
        public SerializableVector3 playerPosition;
    }

    [Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
