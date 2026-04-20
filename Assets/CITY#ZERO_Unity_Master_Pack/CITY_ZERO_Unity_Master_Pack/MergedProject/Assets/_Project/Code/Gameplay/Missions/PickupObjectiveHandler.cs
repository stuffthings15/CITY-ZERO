using System.Collections.Generic;
using UnityEngine;

namespace CityZero.Gameplay.Missions
{
    public sealed class PickupObjectiveHandler : BaseMissionObjectiveHandler
    {
        private readonly HashSet<string> _pickedTargets = new();
        private readonly string _targetId;

        public PickupObjectiveHandler(string targetId)
        {
            _targetId = targetId;
        }

        public override bool CanHandle(string objectiveType) => objectiveType == "pickup";

        public override void Tick(float deltaTime)
        {
        }

        public void NotifyPickedUp(string targetId)
        {
            if (targetId == _targetId)
            {
                _pickedTargets.Add(targetId);
                IsComplete = true;
            }
        }

        public override string GetProgressText() => $"Pick up {_targetId}";
    }
}
