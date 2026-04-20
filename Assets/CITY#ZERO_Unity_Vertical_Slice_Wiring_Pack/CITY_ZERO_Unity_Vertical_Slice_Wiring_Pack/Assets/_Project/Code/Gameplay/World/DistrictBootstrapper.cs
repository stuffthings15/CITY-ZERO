using UnityEngine;

namespace CityZero.Gameplay.World
{
    public sealed class DistrictBootstrapper : MonoBehaviour
    {
        [SerializeField] private string _districtId = "old_quarter";
        [SerializeField] private DistrictSpawnRegistry _spawnRegistry;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private string _initialSpawnId = "safehouse_spawn";

        private bool _bootstrapped;

        private void Start()
        {
            BootstrapDistrict();
        }

        [ContextMenu("Bootstrap District")]
        public void BootstrapDistrict()
        {
            if (_bootstrapped || _spawnRegistry == null || _playerTransform == null)
            {
                return;
            }

            Transform spawn = _spawnRegistry.GetSpawnPoint(_initialSpawnId);
            if (spawn != null)
            {
                _playerTransform.position = spawn.position;
            }

            _bootstrapped = true;
            Debug.Log($"District bootstrapped: {_districtId}");
        }
    }
}
