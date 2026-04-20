using UnityEngine;
using CityZero.Data.RuntimeDTOs;

namespace CityZero.Data.Importers
{
    public sealed class WeaponDatabase : MonoBehaviour
    {
        [SerializeField] private ConfigDatabase _configDatabase;

        public WeaponData Get(string id)
        {
            if (_configDatabase != null && _configDatabase.Weapons.TryGetValue(id, out WeaponData data))
            {
                return data;
            }

            return null;
        }
    }

    public sealed class VehicleDatabase : MonoBehaviour
    {
        [SerializeField] private ConfigDatabase _configDatabase;

        public VehicleData Get(string id)
        {
            if (_configDatabase != null && _configDatabase.Vehicles.TryGetValue(id, out VehicleData data))
            {
                return data;
            }

            return null;
        }
    }
}
