using UnityEngine;
using TMPro;

namespace CityZero.Gameplay.Missions
{
    public sealed class MissionUIBinder : MonoBehaviour
    {
        [SerializeField] private MissionManager _missionManager;
        [SerializeField] private string _trackedMissionId;
        [SerializeField] private TMP_Text _objectiveText;
        [SerializeField] private TMP_Text _stateText;

        private void Update()
        {
            if (_missionManager == null)
            {
                return;
            }

            MissionRuntime runtime = _missionManager.GetMissionRuntime(_trackedMissionId);
            if (runtime == null)
            {
                if (_objectiveText != null) _objectiveText.text = string.Empty;
                if (_stateText != null) _stateText.text = string.Empty;
                return;
            }

            if (_objectiveText != null) _objectiveText.text = runtime.GetObjectiveText();
            if (_stateText != null) _stateText.text = runtime.State.ToString();
        }
    }
}
