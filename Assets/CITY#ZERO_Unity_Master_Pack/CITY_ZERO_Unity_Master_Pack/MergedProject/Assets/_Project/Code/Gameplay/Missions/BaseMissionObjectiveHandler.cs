using CityZero.Data.RuntimeDTOs;

namespace CityZero.Gameplay.Missions
{
    public abstract class BaseMissionObjectiveHandler : IMissionObjectiveHandler
    {
        protected MissionObjectiveData ObjectiveData;

        public bool IsComplete { get; protected set; }
        public bool IsFailed { get; protected set; }

        public abstract bool CanHandle(string objectiveType);

        public virtual void Initialize(MissionObjectiveData objectiveData)
        {
            ObjectiveData = objectiveData;
            IsComplete = false;
            IsFailed = false;
        }

        public abstract void Tick(float deltaTime);

        public virtual string GetProgressText()
        {
            return ObjectiveData?.type ?? "objective";
        }
    }
}
