using UnityEngine;
using TMPro;
using CityZero.Core.Debugging;
using CityZero.Gameplay.Heat;

namespace CityZero.UI.Debug
{
    public sealed class DebugMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private TMP_InputField _commandInput;
        [SerializeField] private TMP_Text _heatLabel;
        [SerializeField] private HeatSystem _heatSystem;
        [SerializeField] private DebugCommandRegistry _registry;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                Toggle();
            }

            if (_panelRoot != null && _panelRoot.activeSelf && Input.GetKeyDown(KeyCode.Return))
            {
                SubmitCommand();
            }

            if (_heatLabel != null && _heatSystem != null)
            {
                _heatLabel.text = $"Heat: {_heatSystem.HeatLevel}";
            }
        }

        public void Toggle()
        {
            if (_panelRoot != null)
            {
                _panelRoot.SetActive(!_panelRoot.activeSelf);
            }
        }

        public void SubmitCommand()
        {
            if (_commandInput == null || _registry == null)
            {
                return;
            }

            _registry.TryExecute(_commandInput.text);
            _commandInput.text = string.Empty;
        }
    }
}
