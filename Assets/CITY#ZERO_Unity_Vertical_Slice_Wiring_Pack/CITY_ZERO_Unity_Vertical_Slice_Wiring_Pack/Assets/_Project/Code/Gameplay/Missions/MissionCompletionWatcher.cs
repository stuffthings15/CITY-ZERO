using UnityEngine;
using CityZero.Core.EventBus;
using CityZero.Data.Importers;
using CityZero.Data.RuntimeDTOs;

namespace CityZero.Gameplay.Missions
{
    public sealed class MissionCompletionWatcher : MonoBehaviour
    {
        [SerializeField] private MissionManagerSafe _missionManager;
        [SerializeField] private MissionRewardProcessor _rewardProcessor;
        [SerializeField] private ConfigDatabase _configDatabase;

        private void OnEnable()
        {
            GameEventBus.MissionStateChanged += OnMissionStateChanged;
        }

        private void OnDisable()
        {
            GameEventBus.MissionStateChanged -= OnMissionStateChanged;
        }

        private void OnMissionStateChanged(MissionStateChangedEvent evt)
        {
            if (evt.State != MissionState.Succeeded || _configDatabase == null || _rewardProcessor == null)
            {
                return;
            }

            if (_configDatabase.Missions.TryGetValue(evt.MissionId, out MissionData mission))
            {
                _rewardProcessor.ApplyRewards(mission);
            }
        }
    }
}
