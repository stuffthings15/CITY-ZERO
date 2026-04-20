using System.Collections.Generic;
using UnityEngine;
using CityZero.Data.Importers;
using CityZero.Data.RuntimeDTOs;

namespace CityZero.Gameplay.Missions
{
    public sealed class MissionManager : MonoBehaviour
    {
        [SerializeField] private ConfigDatabase _configDatabase;

        private readonly Dictionary<string, MissionRuntime> _activeMissions = new();

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            foreach (var pair in _activeMissions)
            {
                pair.Value.Tick(deltaTime);
            }
        }

        public bool TryStartMission(string missionId, List<IMissionObjectiveHandler> handlers)
        {
            if (_configDatabase == null)
            {
                Debug.LogError("MissionManager is missing ConfigDatabase reference.");
                return false;
            }

            if (!_configDatabase.Missions.TryGetValue(missionId, out MissionData data))
            {
                Debug.LogWarning($"Mission {missionId} was not found.");
                return false;
            }

            if (!ConfigValidator.Validate(data, out string error))
            {
                Debug.LogWarning(error);
                return false;
            }

            MissionRuntime runtime = new MissionRuntime(data, handlers);
            runtime.Start();
            _activeMissions[missionId] = runtime;
            return true;
        }

        public MissionRuntime GetMissionRuntime(string missionId)
        {
            _activeMissions.TryGetValue(missionId, out MissionRuntime runtime);
            return runtime;
        }
    }
}
