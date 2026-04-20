using UnityEngine;
using UnityEngine.InputSystem;

namespace CityZero.Gameplay.Player
{
    // Minimal player controller for top-down movement and vehicle enter/exit hooks
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;
        private Vector2 _moveInput;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null) _rb = gameObject.AddComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            if (_rb == null) return;
            Vector2 velocity = _moveInput * moveSpeed;
            _rb.velocity = velocity;
        }

        // Hook for entering a vehicle
        public void EnterVehicle(GameObject vehicle)
        {
            // TODO: implement enter vehicle logic (detach player physics, position, parent, etc.)
            gameObject.SetActive(false);
        }

        // Hook for exiting vehicle
        public void ExitVehicle(Vector3 spawnPosition)
        {
            transform.position = spawnPosition;
            gameObject.SetActive(true);
        }
    }
}
