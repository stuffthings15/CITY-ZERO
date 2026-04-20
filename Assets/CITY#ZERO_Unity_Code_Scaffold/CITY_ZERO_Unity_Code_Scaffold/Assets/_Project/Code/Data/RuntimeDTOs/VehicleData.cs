using System;

namespace CityZero.Data.RuntimeDTOs
{
    [Serializable]
    public sealed class VehicleData
    {
        public string id;
        public string displayName;
        public string @class;
        public string manufacturer;
        public float mass;
        public float maxSpeed;
        public float acceleration;
        public float brakeForce;
        public float steeringResponse;
        public float traction;
        public float durability;
        public float heatSignature;
        public int cargoSlots;
        public int seatCount;
        public float hotwireDifficulty;
        public float spawnWeight;
        public DistrictWeightData[] districtWeights;
        public string[] liveries;
        public string audioProfile;
        public DamageModelData damageModel;
    }

    [Serializable]
    public sealed class DistrictWeightData
    {
        public string districtId;
        public float weight;
    }

    [Serializable]
    public sealed class DamageModelData
    {
        public int engineHealth;
        public int bodyHealth;
        public int wheelHealth;
        public int fireThreshold;
    }
}
