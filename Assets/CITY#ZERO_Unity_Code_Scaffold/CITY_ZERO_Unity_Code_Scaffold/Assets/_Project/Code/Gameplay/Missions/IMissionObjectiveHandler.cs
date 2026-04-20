using CityZero.Data.RuntimeDTOs;

namespace CityZero.Gameplay.Missions
{
    public interface IMissionObjectiveHandler
    {
        bool CanHandle(string objectiveType);
        void Initialize(MissionObjectiveData objectiveData);
        void Tick(float deltaTime);
        bool IsComplete { get; }
        bool IsFailed { get; }
        string GetProgressText();
    }
}
