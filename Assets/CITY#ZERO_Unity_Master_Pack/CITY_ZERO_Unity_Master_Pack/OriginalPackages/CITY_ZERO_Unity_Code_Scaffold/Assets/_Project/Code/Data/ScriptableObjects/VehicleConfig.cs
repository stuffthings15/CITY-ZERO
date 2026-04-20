using UnityEngine;
using CityZero.Data.RuntimeDTOs;

namespace CityZero.Data.ScriptableObjects
{
    [CreateAssetMenu(menuName = "CityZero/Config/Vehicle", fileName = "VehicleConfig")]
    public sealed class VehicleConfig : ScriptableObject
    {
        public VehicleData data;
    }
}
