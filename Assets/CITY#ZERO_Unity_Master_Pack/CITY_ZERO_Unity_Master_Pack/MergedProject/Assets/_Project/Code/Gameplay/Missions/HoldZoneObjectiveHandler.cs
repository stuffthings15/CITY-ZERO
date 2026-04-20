using UnityEngine;

namespace CityZero.Gameplay.Missions
{
    public sealed class HoldZoneObjectiveHandler : BaseMissionObjectiveHandler
    {
        private readonly Transform _player;
        private readonly Vector3 _zoneCenter;
        private readonly float _zoneRadius;
        private readonly float _holdSeconds;
        private float _heldTime;

        public HoldZoneObjectiveHandler(Transform player, Vector3 zoneCenter, float zoneRadius, float holdSeconds)
        {
            _player = player;
            _zoneCenter = zoneCenter;
            _zoneRadius = zoneRadius;
            _holdSeconds = holdSeconds;
        }

        public override bool CanHandle(string objectiveType) => objectiveType == "hold_zone";

        public override void Tick(float deltaTime)
        {
            if (IsComplete || IsFailed || _player == null)
            {
                return;
            }

            float distance = Vector3.Distance(_player.position, _zoneCenter);
            if (distance <= _zoneRadius)
            {
                _heldTime += deltaTime;
                if (_heldTime >= _holdSeconds)
                {
                    IsComplete = true;
                }
            }
            else
            {
                _heldTime = Mathf.Max(0f, _heldTime - deltaTime * 0.5f);
            }
        }

        public override string GetProgressText()
        {
            return $"Hold zone ({_heldTime:0.0}/{_holdSeconds:0.0}s)";
        }
    }
}
