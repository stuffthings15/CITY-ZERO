using UnityEngine;
using CityZero.Core.EventBus;

namespace CityZero.Gameplay.Heat
{
    public sealed class HeatSystem : MonoBehaviour
    {
        [SerializeField] private float _heatScore;
        [SerializeField] private int _heatLevel;
        [SerializeField] private float _decayPerSecond = 4f;

        public int HeatLevel => _heatLevel;
        public float HeatScore => _heatScore;

        private void Update()
        {
            if (_heatScore > 0f)
            {
                _heatScore = Mathf.Max(0f, _heatScore - (_decayPerSecond * Time.deltaTime));
                RefreshHeatLevel();
            }
        }

        public void CommitCrime(int severity, int witnessCount, float districtMultiplier)
        {
            float added = severity * (1f + (witnessCount * 0.25f)) * Mathf.Max(0.1f, districtMultiplier);
            _heatScore += added;
            GameEventBus.Raise(new CrimeCommittedEvent(severity, witnessCount, districtMultiplier));
            RefreshHeatLevel();
        }

        private void RefreshHeatLevel()
        {
            int previous = _heatLevel;
            _heatLevel = CalculateHeatLevel(_heatScore);

            if (previous != _heatLevel)
            {
                GameEventBus.Raise(new HeatLevelChangedEvent(previous, _heatLevel));
            }
        }

        private static int CalculateHeatLevel(float score)
        {
            if (score < 10f) return 0;
            if (score < 25f) return 1;
            if (score < 45f) return 2;
            if (score < 70f) return 3;
            if (score < 100f) return 4;
            return 5;
        }
    }
}
