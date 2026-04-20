using UnityEngine;
using CityZero.Data.RuntimeDTOs;

namespace CityZero.Gameplay.Missions
{
    public sealed class MissionRewardProcessor : MonoBehaviour
    {
        [SerializeField] private int _cash;

        public int Cash => _cash;

        public void ApplyRewards(MissionData mission)
        {
            if (mission == null || mission.rewards == null)
            {
                return;
            }

            _cash += mission.rewards.cash;
            Debug.Log($"Rewards applied for {mission.id}. Cash +{mission.rewards.cash}. Total {_cash}");
        }
    }
}
