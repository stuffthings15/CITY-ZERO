using UnityEngine;
using CityZero.Data.Importers;
using CityZero.Gameplay.Inventory;

namespace CityZero.Core.Bootstrap
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private ConfigDatabase _configDatabase;
        [SerializeField] private SimpleInventorySystem _inventorySystem;

        public static SimpleInventorySystem Inventory { get; private set; }

        private void Awake()
        {
            if (_configDatabase == null)
            {
                Debug.LogError("ConfigDatabase is not assigned on GameBootstrap.");
                return;
            }

            _configDatabase.LoadAll();

            if (_inventorySystem == null)
            {
                _inventorySystem = gameObject.AddComponent<SimpleInventorySystem>();
            }

            Inventory = _inventorySystem;
            DontDestroyOnLoad(gameObject);
        }
    }
}
