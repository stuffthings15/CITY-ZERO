using UnityEngine;

namespace CityZero.Gameplay.Missions
{
    public sealed class ReachLocationObjectiveHandler : BaseMissionObjectiveHandler
    {
        private readonly Transform _playerTransform;
        private readonly Vector3 _targetPosition;
        private readonly float _radius;

        public ReachLocationObjectiveHandler(Transform playerTransform, Vector3 targetPosition, float radius)
        {
            _playerTransform = playerTransform;
            _targetPosition = targetPosition;
            _radius = radius;
        }

        public override bool CanHandle(string objectiveType) => objectiveType == "reach_location";

        public override void Tick(float deltaTime)
        {
            if (IsComplete || IsFailed || _playerTransform == null)
            {
                return;
            }

            float distance = Vector3.Distance(_playerTransform.position, _targetPosition);
            if (distance <= _radius)
            {
                IsComplete = true;
            }
        }

        public override string GetProgressText()
        {
            return $"Reach target area ({_radius:0.0}m)";
        }
    }
}
