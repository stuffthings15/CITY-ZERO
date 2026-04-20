using UnityEngine;
using UnityEngine.InputSystem;

namespace CityZero.Gameplay.Vehicles
{
    public sealed class VehicleDriverInputRelay : MonoBehaviour
    {
        [SerializeField] private VehicleController _vehicleController;

        private Vector2 _moveInput;

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void Update()
        {
            if (_vehicleController == null || !_vehicleController.HasDriver)
            {
                return;
            }

            _vehicleController.SetInput(_moveInput.y, _moveInput.x);
        }
    }
}
