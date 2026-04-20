using System.Collections.Generic;
using UnityEngine;
using CityZero.Data.Importers;
using CityZero.Data.RuntimeDTOs;
using CityZero.Gameplay.Heat;

namespace CityZero.Gameplay.Missions
{
    public sealed class MissionFactory : MonoBehaviour
    {
        [SerializeField] private ConfigDatabase _configDatabase;
        [SerializeField] private HeatSystem _heatSystem;
        [SerializeField] private Transform _playerTransform;

        public List<IMissionObjectiveHandler> CreateHandlers(MissionData mission)
        {
            var handlers = new List<IMissionObjectiveHandler>();

            if (mission == null || mission.objectives == null)
            {
                return handlers;
            }

            foreach (MissionObjectiveData objective in mission.objectives)
            {
                IMissionObjectiveHandler handler = CreateHandler(objective);
                if (handler != null)
                {
                    handler.Initialize(objective);
                    handlers.Add(handler);
                }
                else
                {
                    Debug.LogWarning($"No handler available for objective type {objective.type}");
                }
            }

            return handlers;
        }

        public bool TryStartMission(string missionId, MissionManager missionManager)
        {
            if (_configDatabase == null || missionManager == null)
            {
                return false;
            }

            if (!_configDatabase.Missions.TryGetValue(missionId, out MissionData mission))
            {
                return false;
            }

            return missionManager.TryStartMission(missionId, CreateHandlers(mission));
        }

        private IMissionObjectiveHandler CreateHandler(MissionObjectiveData objective)
        {
            return objective.type switch
            {
                "pickup" => new PickupObjectiveHandler(objective.target),
                "deliver" => new DeliverObjectiveHandler(objective.target, objective.timeLimit),
                "escape_heat" => new EscapeHeatObjectiveHandler(_heatSystem, objective.requiredHeatBelow, objective.timeout),
                "destroy_target" => new DestroyTargetObjectiveHandler(objective.target),
                "hold_zone" => new HoldZoneObjectiveHandler(_playerTransform, Vector3.zero, 3f, objective.timeout > 0 ? objective.timeout : 30f),
                _ => null
            };
        }
    }
}
