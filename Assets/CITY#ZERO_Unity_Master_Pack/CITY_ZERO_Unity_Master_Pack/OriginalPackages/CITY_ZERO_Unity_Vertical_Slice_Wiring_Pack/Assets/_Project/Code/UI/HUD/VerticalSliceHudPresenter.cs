using UnityEngine;
using TMPro;
using CityZero.Gameplay.Missions;
using CityZero.Gameplay.Heat;

namespace CityZero.UI.HUD
{
    public sealed class VerticalSliceHudPresenter : MonoBehaviour
    {
        [SerializeField] private MissionManagerSafe _missionManager;
        [SerializeField] private HeatSystem _heatSystem;
        [SerializeField] private MissionRewardProcessor _rewardProcessor;
        [SerializeField] private string _trackedMissionId = "mq_a1_03_dead_drop_dive";

        [SerializeField] private TMP_Text _missionText;
        [SerializeField] private TMP_Text _heatText;
        [SerializeField] private TMP_Text _cashText;

        private void Update()
        {
            if (_heatText != null && _heatSystem != null)
            {
                _heatText.text = $"HEAT {_heatSystem.HeatLevel}";
            }

            if (_cashText != null && _rewardProcessor != null)
            {
                _cashText.text = $"${_rewardProcessor.Cash}";
            }

            if (_missionText != null && _missionManager != null)
            {
                MissionRuntimeSafe runtime = _missionManager.GetMissionRuntime(_trackedMissionId);
                _missionText.text = runtime != null ? runtime.GetObjectiveText() : "No active mission";
            }
        }
    }
}
