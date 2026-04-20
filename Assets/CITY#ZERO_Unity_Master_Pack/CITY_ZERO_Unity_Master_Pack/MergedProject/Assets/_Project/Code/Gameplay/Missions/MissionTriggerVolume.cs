using UnityEngine;
using CityZero.Gameplay.Player;

namespace CityZero.Gameplay.Missions
{
    [RequireComponent(typeof(Collider))]
    public sealed class MissionTriggerVolume : MonoBehaviour
    {
        [SerializeField] private string _missionId;
        [SerializeField] private MissionFactory _missionFactory;
        [SerializeField] private MissionManager _missionManager;
        [SerializeField] private bool _triggerOnce = true;

        private bool _consumed;

        private void OnTriggerEnter(Collider other)
        {
            if (_consumed && _triggerOnce)
            {
                return;
            }

            if (!other.TryGetComponent(out PlayerController _))
            {
                return;
            }

            if (_missionFactory != null && _missionManager != null && _missionFactory.TryStartMission(_missionId, _missionManager))
            {
                _consumed = true;
            }
        }
    }
}
