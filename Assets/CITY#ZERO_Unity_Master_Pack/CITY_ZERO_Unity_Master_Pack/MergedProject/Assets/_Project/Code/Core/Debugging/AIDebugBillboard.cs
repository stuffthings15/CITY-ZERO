using UnityEngine;
using TMPro;
using CityZero.Gameplay.AI;

namespace CityZero.Core.Debugging
{
    public sealed class AIDebugBillboard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private MonoBehaviour _source;
        [SerializeField] private Camera _camera;

        private void LateUpdate()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_camera != null)
            {
                transform.forward = _camera.transform.forward;
            }

            if (_label == null || _source == null)
            {
                return;
            }

            string text = _source switch
            {
                CivilianAIController civilian => civilian.GetStateName(),
                PoliceAIController police => police.GetStateName(),
                FactionAgentController faction => faction.GetStateName(),
                _ => _source.GetType().Name
            };

            _label.text = text;
        }
    }
}
