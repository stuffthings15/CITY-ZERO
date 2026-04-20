using UnityEngine;

namespace CityZero.Gameplay.Vehicles
{
    public sealed class VehicleSeat : MonoBehaviour
    {
        [SerializeField] private Transform _seatRoot;
        [SerializeField] private Transform _leftExitPoint;
        [SerializeField] private Transform _rightExitPoint;

        public Transform SeatRoot => _seatRoot != null ? _seatRoot : transform;
        public Transform LeftExitPoint => _leftExitPoint;
        public Transform RightExitPoint => _rightExitPoint;

        public Vector3 GetBestExitPoint(Vector3 playerWorldPosition)
        {
            if (_leftExitPoint == null && _rightExitPoint == null)
            {
                return transform.position - transform.right * 1.5f;
            }

            if (_leftExitPoint == null) return _rightExitPoint.position;
            if (_rightExitPoint == null) return _leftExitPoint.position;

            float leftDistance = Vector3.Distance(playerWorldPosition, _leftExitPoint.position);
            float rightDistance = Vector3.Distance(playerWorldPosition, _rightExitPoint.position);
            return leftDistance <= rightDistance ? _leftExitPoint.position : _rightExitPoint.position;
        }
    }
}
