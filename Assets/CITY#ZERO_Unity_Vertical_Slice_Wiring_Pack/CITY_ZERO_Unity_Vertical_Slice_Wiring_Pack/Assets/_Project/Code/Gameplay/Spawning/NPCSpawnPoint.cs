using UnityEngine;

namespace CityZero.Gameplay.Spawning
{
    public sealed class NPCSpawnPoint : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private bool _spawnOnStart = true;
        [SerializeField] private Transform _parent;

        private GameObject _instance;

        private void Start()
        {
            if (_spawnOnStart)
            {
                Spawn();
            }
        }

        [ContextMenu("Spawn")]
        public void Spawn()
        {
            if (_prefab == null || _instance != null)
            {
                return;
            }

            _instance = Instantiate(_prefab, transform.position, transform.rotation, _parent);
        }
    }
}
