using UnityEngine;
using CityZero.Data.RuntimeDTOs;

namespace CityZero.Data.ScriptableObjects
{
    [CreateAssetMenu(menuName = "CityZero/Config/Weapon", fileName = "WeaponConfig")]
    public sealed class WeaponConfig : ScriptableObject
    {
        public WeaponData data;
    }
}
