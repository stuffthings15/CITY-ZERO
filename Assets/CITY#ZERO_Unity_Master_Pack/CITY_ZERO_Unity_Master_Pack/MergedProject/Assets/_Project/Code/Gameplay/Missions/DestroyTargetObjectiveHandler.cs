namespace CityZero.Gameplay.Missions
{
    public sealed class DestroyTargetObjectiveHandler : BaseMissionObjectiveHandler
    {
        private readonly string _targetId;

        public DestroyTargetObjectiveHandler(string targetId)
        {
            _targetId = targetId;
        }

        public override bool CanHandle(string objectiveType) => objectiveType == "destroy_target";

        public override void Tick(float deltaTime)
        {
        }

        public void NotifyDestroyed(string targetId)
        {
            if (targetId == _targetId)
            {
                IsComplete = true;
            }
        }

        public override string GetProgressText() => $"Destroy {_targetId}";
    }
}
