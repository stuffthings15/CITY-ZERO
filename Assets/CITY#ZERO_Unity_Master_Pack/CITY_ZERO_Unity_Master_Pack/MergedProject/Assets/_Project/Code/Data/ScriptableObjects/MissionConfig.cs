using UnityEngine;
using CityZero.Data.RuntimeDTOs;

namespace CityZero.Data.ScriptableObjects
{
    [CreateAssetMenu(menuName = "CityZero/Config/Mission", fileName = "MissionConfig")]
    public sealed class MissionConfig : ScriptableObject
    {
        public MissionData data;
    }
}
