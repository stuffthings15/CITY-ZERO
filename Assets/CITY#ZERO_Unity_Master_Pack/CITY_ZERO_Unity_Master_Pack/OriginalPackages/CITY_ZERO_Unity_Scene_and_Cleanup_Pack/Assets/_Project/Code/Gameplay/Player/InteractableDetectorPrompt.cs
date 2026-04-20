using UnityEngine;
using CityZero.Gameplay.Interaction;

namespace CityZero.Gameplay.Player
{
    [RequireComponent(typeof(SphereCollider))]
    public sealed class InteractableDetectorPrompt : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private InteractionPromptPresenter _promptPresenter;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IInteractable interactable))
            {
                _playerController?.SetNearbyInteractable(interactable);
                _promptPresenter?.SetCurrent(interactable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IInteractable _))
            {
                _playerController?.SetNearbyInteractable(null);
                _promptPresenter?.SetCurrent(null);
            }
        }
    }
}
