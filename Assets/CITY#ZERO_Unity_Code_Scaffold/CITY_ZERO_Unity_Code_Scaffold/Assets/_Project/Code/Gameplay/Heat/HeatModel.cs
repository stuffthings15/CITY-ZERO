using System;

namespace CityZero.Gameplay.Heat
{
    // Pure data model for heat logic to allow unit testing outside Unity
    public sealed class HeatModel
    {
        private float _heatScore;
        private int _heatLevel;
        private readonly float _decayPerSecond;

        public HeatModel(float decayPerSecond = 4f)
        {
            _decayPerSecond = decayPerSecond;
        }

        public int HeatLevel => _heatLevel;
        public float HeatScore => _heatScore;

        public void UpdateDecay(float deltaTime)
        {
            if (_heatScore > 0f)
            {
                _heatScore = Math.Max(0f, _heatScore - (_decayPerSecond * deltaTime));
                RefreshHeatLevel();
            }
        }

        public void CommitCrime(int severity, int witnessCount, float districtMultiplier)
        {
            float added = severity * (1f + (witnessCount * 0.25f)) * Math.Max(0.1f, districtMultiplier);
            _heatScore += added;
            RefreshHeatLevel();
        }

        private void RefreshHeatLevel()
        {
            int previous = _heatLevel;
            _heatLevel = CalculateHeatLevel(_heatScore);
            // Note: Events are raised by the MonoBehaviour adapter (HeatSystem)
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
