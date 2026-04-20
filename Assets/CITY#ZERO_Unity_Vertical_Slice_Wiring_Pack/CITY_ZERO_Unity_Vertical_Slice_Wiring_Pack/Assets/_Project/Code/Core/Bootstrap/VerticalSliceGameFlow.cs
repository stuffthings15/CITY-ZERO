using UnityEngine;
using CityZero.Core.SaveLoad;
using CityZero.Gameplay.Missions;

namespace CityZero.Core.Bootstrap
{
    public sealed class VerticalSliceGameFlow : MonoBehaviour
    {
        [SerializeField] private MissionFactorySafe _missionFactory;
        [SerializeField] private MissionManagerSafe _missionManager;
        [SerializeField] private SaveManager _saveManager;
        [SerializeField] private string _firstMissionId = "mq_a1_03_dead_drop_dive";
        [SerializeField] private bool _autoStartFirstMission;

        private void Start()
        {
            if (_autoStartFirstMission)
            {
                StartFirstMission();
            }
        }

        [ContextMenu("Start First Mission")]
        public void StartFirstMission()
        {
            if (_missionFactory != null && _missionManager != null)
            {
                _missionFactory.TryStartMission(_firstMissionId, _missionManager);
            }
        }

        [ContextMenu("Save Progress")]
        public void SaveProgress()
        {
            _saveManager?.SaveNow();
        }
    }
}
