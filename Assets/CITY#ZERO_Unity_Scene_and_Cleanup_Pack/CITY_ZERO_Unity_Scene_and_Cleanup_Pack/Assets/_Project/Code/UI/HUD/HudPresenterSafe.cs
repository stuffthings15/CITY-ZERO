using UnityEngine;
using TMPro;
using CityZero.Gameplay.Combat;
using CityZero.Gameplay.Heat;
using CityZero.Gameplay.Missions;

namespace CityZero.UI.HUD
{
    public sealed class HudPresenterSafe : MonoBehaviour
    {
        [SerializeField] private HealthComponent _playerHealth;
        [SerializeField] private HeatSystem _heatSystem;
        [SerializeField] private MissionManagerSafe _missionManager;
        [SerializeField] private string _trackedMissionId;

        [Header("UI")]
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private TMP_Text _heatText;
        [SerializeField] private TMP_Text _objectiveText;

        private void Update()
        {
            if (_playerHealth != null && _healthText != null)
            {
                _healthText.text = $"HP {_playerHealth.CurrentHealth}/{_playerHealth.MaxHealth}";
            }

            if (_heatSystem != null && _heatText != null)
            {
                _heatText.text = $"HEAT {_heatSystem.HeatLevel}";
            }

            if (_missionManager != null && _objectiveText != null && !string.IsNullOrWhiteSpace(_trackedMissionId))
            {
                MissionRuntimeSafe runtime = _missionManager.GetMissionRuntime(_trackedMissionId);
                _objectiveText.text = runtime != null ? runtime.GetObjectiveText() : string.Empty;
            }
        }
    }
}
