using System.Collections.Generic;
using UnityEngine;
using CityZero.Data.Importers;
using CityZero.Data.RuntimeDTOs;

namespace CityZero.Gameplay.Missions
{
    public sealed class MissionManagerSafe : MonoBehaviour
    {
        [SerializeField] private ConfigDatabase _configDatabase;

        private readonly Dictionary<string, MissionRuntimeSafe> _activeMissions = new();

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            foreach (var pair in _activeMissions)
            {
                pair.Value.Tick(deltaTime);
            }
        }

        private void OnDestroy()
        {
            foreach (var runtime in _activeMissions.Values)
            {
                runtime.Dispose();
            }
            _activeMissions.Clear();
        }

        public bool TryStartMission(string missionId, List<IMissionObjectiveHandler> handlers)
        {
            if (_configDatabase == null)
            {
                Debug.LogError("MissionManagerSafe is missing ConfigDatabase reference.");
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

            if (_activeMissions.ContainsKey(missionId))
            {
                Debug.LogWarning($"Mission {missionId} is already active.");
                return false;
            }

            MissionRuntimeSafe runtime = new MissionRuntimeSafe(data, handlers);
            runtime.Start();
            _activeMissions[missionId] = runtime;
            return true;
        }

        public MissionRuntimeSafe GetMissionRuntime(string missionId)
        {
            _activeMissions.TryGetValue(missionId, out MissionRuntimeSafe runtime);
            return runtime;
        }
    }
}
