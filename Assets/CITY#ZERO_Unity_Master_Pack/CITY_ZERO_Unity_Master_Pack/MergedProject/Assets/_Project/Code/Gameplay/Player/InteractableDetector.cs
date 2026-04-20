using UnityEngine;

namespace CityZero.Gameplay.Player
{
    [RequireComponent(typeof(SphereCollider))]
    public sealed class InteractableDetector : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IInteractable interactable))
            {
                _playerController?.SetNearbyInteractable(interactable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IInteractable _))
            {
                _playerController?.SetNearbyInteractable(null);
            }
        }
    }
}
