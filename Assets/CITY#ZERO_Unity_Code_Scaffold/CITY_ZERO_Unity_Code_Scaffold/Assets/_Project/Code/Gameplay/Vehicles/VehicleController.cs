using UnityEngine;
using CityZero.Gameplay.Player;

namespace CityZero.Gameplay.Vehicles
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class VehicleController : MonoBehaviour, IInteractable
    {
        [Header("Handling")]
        [SerializeField] private float _acceleration = 18f;
        [SerializeField] private float _brakeForce = 24f;
        [SerializeField] private float _maxSpeed = 22f;
        [SerializeField] private float _steeringSpeed = 120f;
        [SerializeField] private float _traction = 8f;

        private Rigidbody _rigidbody;
        private PlayerController _driver;
        private float _throttleInput;
        private float _steeringInput;

        public bool HasDriver => _driver != null;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _rigidbody.centerOfMass = new Vector3(0f, -0.5f, 0f);
        }

        private void FixedUpdate()
        {
            if (_driver == null)
            {
                return;
            }

            UpdateVehiclePhysics();
        }

        public void SetInput(float throttle, float steering)
        {
            _throttleInput = Mathf.Clamp(throttle, -1f, 1f);
            _steeringInput = Mathf.Clamp(steering, -1f, 1f);
        }

        public void SetDriver(PlayerController driver)
        {
            _driver = driver;
        }

        public void RemoveDriver()
        {
            _driver = null;
            _throttleInput = 0f;
            _steeringInput = 0f;
        }

        public void Interact(PlayerController player)
        {
            if (HasDriver || player == null)
            {
                return;
            }

            player.EnterVehicle(this);
        }

        public string GetInteractionLabel()
        {
            return HasDriver ? string.Empty : "Enter Vehicle";
        }

        private void UpdateVehiclePhysics()
        {
            Vector3 forward = transform.forward;
            Vector3 velocity = _rigidbody.linearVelocity;
            Vector3 forwardVelocity = Vector3.Project(velocity, forward);
            Vector3 lateralVelocity = velocity - forwardVelocity;

            float speed = forwardVelocity.magnitude;
            float signedSpeed = Vector3.Dot(forwardVelocity, forward);

            if (_throttleInput > 0f)
            {
                _rigidbody.AddForce(forward * (_throttleInput * _acceleration), ForceMode.Acceleration);
            }
            else if (_throttleInput < 0f)
            {
                _rigidbody.AddForce(forward * (_throttleInput * _brakeForce), ForceMode.Acceleration);
            }

            if (speed < _maxSpeed || Mathf.Sign(_throttleInput) != Mathf.Sign(signedSpeed))
            {
                float steerAmount = _steeringInput * _steeringSpeed * Time.fixedDeltaTime * Mathf.Clamp01(speed / 3f + 0.2f);
                Quaternion deltaRotation = Quaternion.Euler(0f, steerAmount, 0f);
                _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
            }

            _rigidbody.AddForce(-lateralVelocity * _traction, ForceMode.Acceleration);

            Vector3 planarVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
            if (planarVelocity.magnitude > _maxSpeed)
            {
                planarVelocity = planarVelocity.normalized * _maxSpeed;
                _rigidbody.linearVelocity = new Vector3(planarVelocity.x, _rigidbody.linearVelocity.y, planarVelocity.z);
            }
        }
    }
}
