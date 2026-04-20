using UnityEngine;
using UnityEngine.InputSystem;

namespace CityZero.Gameplay.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public sealed class PlayerInputBridge : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;

        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        public void OnMove(InputAction.CallbackContext context) => _playerController?.OnMove(context);
        public void OnLook(InputAction.CallbackContext context) => _playerController?.OnLook(context);
        public void OnSprint(InputAction.CallbackContext context) => _playerController?.OnSprint(context);
        public void OnRoll(InputAction.CallbackContext context) => _playerController?.OnRoll(context);
        public void OnInteract(InputAction.CallbackContext context) => _playerController?.OnInteract(context);
        public void OnFire(InputAction.CallbackContext context) => _playerController?.OnFire(context);

        public void SwitchToPlayerMap()
        {
            _playerInput?.SwitchCurrentActionMap("Player");
        }

        public void SwitchToVehicleMap()
        {
            _playerInput?.SwitchCurrentActionMap("Vehicle");
        }

        public void SwitchToUIMap()
        {
            _playerInput?.SwitchCurrentActionMap("UI");
        }
    }
}
