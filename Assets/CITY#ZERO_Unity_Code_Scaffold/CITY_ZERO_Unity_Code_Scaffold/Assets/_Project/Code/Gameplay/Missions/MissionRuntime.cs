using System.Collections.Generic;
using CityZero.Core.EventBus;
using CityZero.Data.RuntimeDTOs;

namespace CityZero.Gameplay.Missions
{
    public sealed class MissionRuntime
    {
        private readonly MissionData _missionData;
        private readonly List<IMissionObjectiveHandler> _handlers;

        private int _currentObjectiveIndex;

        public MissionState State { get; private set; }
        public MissionData Data => _missionData;

        public MissionRuntime(MissionData missionData, List<IMissionObjectiveHandler> handlers)
        {
            _missionData = missionData;
            _handlers = handlers;
            _currentObjectiveIndex = 0;
            State = MissionState.Inactive;
        }

        public void Start()
        {
            if (_handlers.Count == 0)
            {
                State = MissionState.Failed;
                GameEventBus.Raise(new MissionStateChangedEvent(_missionData.id, State));
                return;
            }

            State = MissionState.Active;
            GameEventBus.Raise(new MissionStateChangedEvent(_missionData.id, State));
        }

        public void Tick(float deltaTime)
        {
            if (State != MissionState.Active)
            {
                return;
            }

            if (_currentObjectiveIndex < 0 || _currentObjectiveIndex >= _handlers.Count)
            {
                CompleteMission();
                return;
            }

            IMissionObjectiveHandler handler = _handlers[_currentObjectiveIndex];
            handler.Tick(deltaTime);

            if (handler.IsFailed)
            {
                FailMission();
                return;
            }

            if (handler.IsComplete)
            {
                _currentObjectiveIndex++;
                if (_currentObjectiveIndex >= _handlers.Count)
                {
                    CompleteMission();
                }
            }
        }

        public string GetObjectiveText()
        {
            if (State != MissionState.Active || _currentObjectiveIndex < 0 || _currentObjectiveIndex >= _handlers.Count)
            {
                return string.Empty;
            }

            return _handlers[_currentObjectiveIndex].GetProgressText();
        }

        private void CompleteMission()
        {
            State = MissionState.Succeeded;
            GameEventBus.Raise(new MissionStateChangedEvent(_missionData.id, State));
        }

        private void FailMission()
        {
            State = MissionState.Failed;
            GameEventBus.Raise(new MissionStateChangedEvent(_missionData.id, State));
        }
    }
}
