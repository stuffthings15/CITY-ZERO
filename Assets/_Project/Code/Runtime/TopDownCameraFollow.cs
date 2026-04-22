using UnityEngine;

/// <summary>Smoothly follows a target from directly above (GTA 2 style).</summary>
public class TopDownCameraFollow : MonoBehaviour
{
    private Transform _target;
    private float     _height;
    public float      smoothSpeed = 6f;

    public void SetTarget(Transform t) { _target = t; _height = transform.position.y; }

    private void LateUpdate()
    {
        if (_target == null) return;
        var desired = new Vector3(_target.position.x, _height, _target.position.z);
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smoothSpeed);
    }
}
