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
            // Ensure a ConfigDatabase exists at runtime so the bootstrap can run in a clean demo scene
            if (_configDatabase == null)
            {
                _configDatabase = gameObject.GetComponent<ConfigDatabase>();
                if (_configDatabase == null)
                {
                    _configDatabase = gameObject.AddComponent<ConfigDatabase>();
                    Debug.Log("ConfigDatabase was not assigned; created a runtime placeholder on GameBootstrap.");
                }
            }

            // Load configuration (will be empty if no TextAssets are assigned)
            try
            {
                _configDatabase.LoadAll();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"ConfigDatabase.LoadAll() threw an exception: {ex.Message}");
            }

            if (_inventorySystem == null)
            {
                _inventorySystem = gameObject.AddComponent<SimpleInventorySystem>();
            }

            Inventory = _inventorySystem;
            DontDestroyOnLoad(gameObject);
        }
    }
}
