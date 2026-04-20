using UnityEngine;

namespace CityZero.Gameplay.Spawning
{
    public sealed class SpawnPointGroup : MonoBehaviour
    {
        [SerializeField] private NPCSpawnPoint[] _points;

        [ContextMenu("Spawn All")]
        public void SpawnAll()
        {
            if (_points == null)
            {
                return;
            }

            foreach (NPCSpawnPoint point in _points)
            {
                if (point != null)
                {
                    point.Spawn();
                }
            }
        }
    }
}
