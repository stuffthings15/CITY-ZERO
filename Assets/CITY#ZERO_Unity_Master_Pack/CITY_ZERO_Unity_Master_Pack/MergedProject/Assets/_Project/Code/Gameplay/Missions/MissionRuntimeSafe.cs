using System.Collections.Generic;
using CityZero.Core.EventBus;
using CityZero.Data.RuntimeDTOs;

namespace CityZero.Gameplay.Missions
{
    public sealed class MissionRuntimeSafe
    {
        private readonly MissionData _missionData;
        private readonly List<IMissionObjectiveHandler> _handlers;
        private MissionEventRelay _relay;
        private int _currentObjectiveIndex;

        public MissionState State { get; private set; }
        public MissionData Data => _missionData;

        public MissionRuntimeSafe(MissionData missionData, List<IMissionObjectiveHandler> handlers)
        {
            _missionData = missionData;
            _handlers = handlers ?? new List<IMissionObjectiveHandler>();
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

            _relay = new MissionEventRelay(_handlers);
            State = MissionState.Active;
            GameEventBus.Raise(new MissionStateChangedEvent(_missionData.id, State));
        }

        public void Tick(float deltaTime)
        {
            if (State != MissionState.Active)
            {
                return;
            }

            if (_currentObjectiveIndex >= _handlers.Count)
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

        public void Dispose()
        {
            _relay?.Dispose();
            _relay = null;
        }

        private void CompleteMission()
        {
            State = MissionState.Succeeded;
            Dispose();
            GameEventBus.Raise(new MissionStateChangedEvent(_missionData.id, State));
        }

        private void FailMission()
        {
            State = MissionState.Failed;
            Dispose();
            GameEventBus.Raise(new MissionStateChangedEvent(_missionData.id, State));
        }
    }
}
