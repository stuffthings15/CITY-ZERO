using CityZero.Gameplay.Heat;

namespace CityZero.Gameplay.Missions
{
    public sealed class EscapeHeatObjectiveHandler : BaseMissionObjectiveHandler
    {
        private readonly HeatSystem _heatSystem;
        private readonly int _requiredHeatBelow;
        private readonly float _timeoutSeconds;
        private float _elapsed;

        public EscapeHeatObjectiveHandler(HeatSystem heatSystem, int requiredHeatBelow, float timeoutSeconds)
        {
            _heatSystem = heatSystem;
            _requiredHeatBelow = requiredHeatBelow;
            _timeoutSeconds = timeoutSeconds;
        }

        public override bool CanHandle(string objectiveType) => objectiveType == "escape_heat";

        public override void Tick(float deltaTime)
        {
            if (IsComplete || IsFailed || _heatSystem == null)
            {
                return;
            }

            _elapsed += deltaTime;

            if (_heatSystem.HeatLevel < _requiredHeatBelow)
            {
                IsComplete = true;
                return;
            }

            if (_timeoutSeconds > 0f && _elapsed > _timeoutSeconds)
            {
                IsFailed = true;
            }
        }

        public override string GetProgressText()
        {
            float remaining = _timeoutSeconds > 0f ? (_timeoutSeconds - _elapsed) : 0f;
            return _timeoutSeconds > 0f
                ? $"Lose heat below {_requiredHeatBelow} ({remaining:0}s)"
                : $"Lose heat below {_requiredHeatBelow}";
        }
    }
}
