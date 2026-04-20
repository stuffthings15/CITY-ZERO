using UnityEngine;

namespace CityZero.Gameplay.Traffic
{
    public sealed class VehicleSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] _vehiclePrefabs;
        [SerializeField] private int _count = 5;
        [SerializeField] private float _spawnRadius = 20f;

        [ContextMenu("Spawn Vehicles")]
        public void SpawnVehicles()
        {
            if (_vehiclePrefabs == null || _vehiclePrefabs.Length == 0)
            {
                return;
            }

            for (int i = 0; i < _count; i++)
            {
                Vector2 circle = Random.insideUnitCircle * _spawnRadius;
                Vector3 position = transform.position + new Vector3(circle.x, 0f, circle.y);
                int prefabIndex = Random.Range(0, _vehiclePrefabs.Length);
                Instantiate(_vehiclePrefabs[prefabIndex], position, Quaternion.identity);
            }
        }
    }
}
