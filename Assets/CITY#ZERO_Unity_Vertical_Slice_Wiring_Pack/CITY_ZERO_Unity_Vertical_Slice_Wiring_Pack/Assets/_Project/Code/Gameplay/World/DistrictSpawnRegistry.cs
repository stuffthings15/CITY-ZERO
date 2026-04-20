using System.Collections.Generic;
using UnityEngine;

namespace CityZero.Gameplay.World
{
    public sealed class DistrictSpawnRegistry : MonoBehaviour
    {
        [SerializeField] private SpawnPointEntry[] _spawnPoints;

        private Dictionary<string, Transform> _cache;

        private void Awake()
        {
            _cache = new Dictionary<string, Transform>();

            if (_spawnPoints == null)
            {
                return;
            }

            foreach (SpawnPointEntry entry in _spawnPoints)
            {
                if (entry != null && !string.IsNullOrWhiteSpace(entry.Id) && entry.Point != null)
                {
                    _cache[entry.Id] = entry.Point;
                }
            }
        }

        public Transform GetSpawnPoint(string id)
        {
            if (_cache == null)
            {
                Awake();
            }

            return !string.IsNullOrWhiteSpace(id) && _cache.TryGetValue(id, out Transform point) ? point : null;
        }
    }

    [System.Serializable]
    public sealed class SpawnPointEntry
    {
        [SerializeField] private string _id;
        [SerializeField] private Transform _point;

        public string Id => _id;
        public Transform Point => _point;
    }
}
