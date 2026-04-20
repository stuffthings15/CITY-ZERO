using UnityEngine;

namespace CityZero.Gameplay.Missions
{
    public sealed class DeliverObjectiveHandler : BaseMissionObjectiveHandler
    {
        private readonly string _targetId;
        private readonly float _timeLimitSeconds;
        private float _elapsed;

        public DeliverObjectiveHandler(string targetId, float timeLimitSeconds)
        {
            _targetId = targetId;
            _timeLimitSeconds = timeLimitSeconds;
        }

        public override bool CanHandle(string objectiveType) => objectiveType == "deliver";

        public override void Tick(float deltaTime)
        {
            if (IsComplete || IsFailed)
            {
                return;
            }

            if (_timeLimitSeconds > 0f)
            {
                _elapsed += deltaTime;
                if (_elapsed > _timeLimitSeconds)
                {
                    IsFailed = true;
                }
            }
        }

        public void NotifyDelivered(string targetId)
        {
            if (targetId == _targetId)
            {
                IsComplete = true;
            }
        }

        public override string GetProgressText()
        {
            if (_timeLimitSeconds > 0f)
            {
                float remaining = Mathf.Max(0f, _timeLimitSeconds - _elapsed);
                return $"Deliver to {_targetId} ({remaining:0}s)";
            }

            return $"Deliver to {_targetId}";
        }
    }
}
