using UnityEngine;
using CityZero.Data.Importers;

namespace CityZero.Core.Bootstrap
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private ConfigDatabase _configDatabase;

        private void Awake()
        {
            if (_configDatabase == null)
            {
                Debug.LogError("ConfigDatabase is not assigned on GameBootstrap.");
                return;
            }

            _configDatabase.LoadAll();
            DontDestroyOnLoad(gameObject);
        }
    }
}
