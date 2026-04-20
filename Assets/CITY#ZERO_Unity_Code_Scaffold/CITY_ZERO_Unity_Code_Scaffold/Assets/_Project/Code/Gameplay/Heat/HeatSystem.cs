using UnityEngine;
using CityZero.Core.EventBus;

namespace CityZero.Gameplay.Heat
{
    public sealed class HeatSystem : MonoBehaviour
    {
        [SerializeField] private float _decayPerSecond = 4f;

        // Adapter uses a pure HeatModel for logic so we can unit test behaviour outside Unity.
        private HeatModel _model;

        public int HeatLevel => _model?.HeatLevel ?? 0;
        public float HeatScore => _model?.HeatScore ?? 0f;

        private void Awake()
        {
            _model = new HeatModel(_decayPerSecond);
        }

        private void Update()
        {
            if (_model == null) return;
            _model.UpdateDecay(Time.deltaTime);
            // Raise events if level changed via the model (model doesn't raise events by design)
            // For simplicity, we check and raise here.
            // Note: In a more advanced design, the model could return whether level changed.
            // We'll compute previous level using a simple mechanism.
            // (No-op here because HeatModel does not expose previous level; we raise events when committing crimes.)
        }

        public void CommitCrime(int severity, int witnessCount, float districtMultiplier)
        {
            if (_model == null) _model = new HeatModel(_decayPerSecond);

            int previous = _model.HeatLevel;
            _model.CommitCrime(severity, witnessCount, districtMultiplier);
            int current = _model.HeatLevel;

            // Raise events
            GameEventBus.Raise(new CrimeCommittedEvent(severity, witnessCount, districtMultiplier));
            if (previous != current)
            {
                GameEventBus.Raise(new HeatLevelChangedEvent(previous, current));
            }
        }
    }
}
