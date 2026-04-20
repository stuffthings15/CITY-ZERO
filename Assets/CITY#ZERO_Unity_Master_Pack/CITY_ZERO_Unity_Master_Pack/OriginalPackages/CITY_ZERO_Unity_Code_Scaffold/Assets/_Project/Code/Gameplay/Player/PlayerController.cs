using UnityEngine;
using UnityEngine.InputSystem;
using CityZero.Gameplay.Combat;
using CityZero.Gameplay.Vehicles;

namespace CityZero.Gameplay.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveAcceleration = 35f;
        [SerializeField] private float _maxMoveSpeed = 7f;
        [SerializeField] private float _sprintMultiplier = 1.35f;
        [SerializeField] private float _drag = 8f;

        [Header("Roll")]
        [SerializeField] private float _rollImpulse = 10f;
        [SerializeField] private float _rollCooldown = 0.75f;

        [Header("References")]
        [SerializeField] private Transform _aimPivot;
        [SerializeField] private WeaponMotor _weaponMotor;

        private Rigidbody _rigidbody;
        private Camera _mainCamera;
        private IInteractable _nearbyInteractable;
        private VehicleController _currentVehicle;

        private Vector2 _moveInput;
        private Vector2 _lookScreenPosition;
        private bool _sprintHeld;
        private float _nextRollTime;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _mainCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            if (_currentVehicle != null)
            {
                return;
            }

            UpdateMovementPhysics();
        }

        private void Update()
        {
            if (_currentVehicle != null)
            {
                return;
            }

            UpdateAimRotation();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            _lookScreenPosition = context.ReadValue<Vector2>();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            _sprintHeld = context.ReadValueAsButton();
        }

        public void OnRoll(InputAction.CallbackContext context)
        {
            if (!context.performed || Time.time < _nextRollTime || _currentVehicle != null)
            {
                return;
            }

            Vector3 inputWorld = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
            if (inputWorld.sqrMagnitude <= 0.01f)
            {
                inputWorld = transform.forward;
            }

            _rigidbody.AddForce(inputWorld * _rollImpulse, ForceMode.VelocityChange);
            _nextRollTime = Time.time + _rollCooldown;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            if (_currentVehicle != null)
            {
                ExitVehicle();
                return;
            }

            _nearbyInteractable?.Interact(this);
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (_currentVehicle != null)
            {
                return;
            }

            if (_weaponMotor == null)
            {
                return;
            }

            if (context.performed)
            {
                _weaponMotor.FirePressed();
            }
            else if (context.canceled)
            {
                _weaponMotor.FireReleased();
            }
        }

        public void SetNearbyInteractable(IInteractable interactable)
        {
            _nearbyInteractable = interactable;
        }

        public void EnterVehicle(VehicleController vehicle)
        {
            if (vehicle == null)
            {
                return;
            }

            _currentVehicle = vehicle;
            vehicle.SetDriver(this);
            gameObject.SetActive(false);
        }

        public void ExitVehicle()
        {
            if (_currentVehicle == null)
            {
                return;
            }

            Vector3 exitPosition = _currentVehicle.transform.position - _currentVehicle.transform.right * 1.5f;
            VehicleController previousVehicle = _currentVehicle;
            _currentVehicle = null;

            transform.position = exitPosition;
            gameObject.SetActive(true);
            previousVehicle.RemoveDriver();
        }

        private void UpdateMovementPhysics()
        {
            Vector3 desiredMove = new Vector3(_moveInput.x, 0f, _moveInput.y);
            if (desiredMove.sqrMagnitude > 1f)
            {
                desiredMove.Normalize();
            }

            float speedMultiplier = _sprintHeld ? _sprintMultiplier : 1f;
            Vector3 desiredVelocity = desiredMove * (_maxMoveSpeed * speedMultiplier);
            Vector3 planarVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
            Vector3 delta = desiredVelocity - planarVelocity;

            _rigidbody.AddForce(delta * _moveAcceleration, ForceMode.Acceleration);
            _rigidbody.AddForce(-planarVelocity * _drag, ForceMode.Acceleration);
        }

        private void UpdateAimRotation()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null)
                {
                    return;
                }
            }

            Ray ray = _mainCamera.ScreenPointToRay(_lookScreenPosition);
            Plane plane = new Plane(Vector3.up, transform.position);

            if (plane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 direction = hitPoint - transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.001f)
                {
                    Quaternion rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
                    transform.rotation = rotation;

                    if (_aimPivot != null)
                    {
                        _aimPivot.rotation = rotation;
                    }
                }
            }
        }
    }
}
