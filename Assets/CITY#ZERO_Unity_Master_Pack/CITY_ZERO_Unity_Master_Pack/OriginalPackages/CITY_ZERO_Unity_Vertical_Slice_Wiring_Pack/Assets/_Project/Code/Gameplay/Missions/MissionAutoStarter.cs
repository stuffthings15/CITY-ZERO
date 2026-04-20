using UnityEngine;

namespace CityZero.Gameplay.Missions
{
    public sealed class MissionAutoStarter : MonoBehaviour
    {
        [SerializeField] private string _missionId = "mq_a1_03_dead_drop_dive";
        [SerializeField] private MissionFactorySafe _missionFactory;
        [SerializeField] private MissionManagerSafe _missionManager;
        [SerializeField] private bool _startOnAwake;

        private void Start()
        {
            if (_startOnAwake)
            {
                StartMission();
            }
        }

        [ContextMenu("Start Mission")]
        public void StartMission()
        {
            if (_missionFactory != null && _missionManager != null)
            {
                _missionFactory.TryStartMission(_missionId, _missionManager);
            }
        }
    }
}
