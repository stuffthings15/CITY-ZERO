using UnityEngine;

/// <summary>Simple WASD mover used when PlayerController is not yet wired up.</summary>
public class FallbackPlayerMover : MonoBehaviour
{
    public float speed = 12f;
    private Rigidbody _rb;

    private void Awake() => _rb = GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        _rb.velocity = new Vector3(h, 0f, v).normalized * speed;
    }
}
