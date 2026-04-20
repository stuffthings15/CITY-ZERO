using UnityEngine;
using TMPro;
using CityZero.Gameplay.Player;

namespace CityZero.Gameplay.Interaction
{
    public sealed class InteractionPromptPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private GameObject _root;

        private IInteractable _current;

        public void SetCurrent(IInteractable interactable)
        {
            _current = interactable;

            bool active = _current != null;
            if (_root != null)
            {
                _root.SetActive(active);
            }

            if (active && _label != null)
            {
                _label.text = _current.GetInteractionLabel();
            }
        }
    }
}
