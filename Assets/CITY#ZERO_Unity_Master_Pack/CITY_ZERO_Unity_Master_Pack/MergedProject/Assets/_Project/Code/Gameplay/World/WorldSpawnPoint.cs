using UnityEngine;

namespace CityZero.Gameplay.World
{
    public sealed class WorldSpawnPoint : MonoBehaviour
    {
        [SerializeField] private string _spawnTag;

        public string SpawnTag => _spawnTag;
    }
}
