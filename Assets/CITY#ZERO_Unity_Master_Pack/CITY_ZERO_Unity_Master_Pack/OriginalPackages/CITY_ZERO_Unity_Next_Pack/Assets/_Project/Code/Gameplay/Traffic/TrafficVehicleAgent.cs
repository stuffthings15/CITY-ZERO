using UnityEngine;
using CityZero.Gameplay.Vehicles;

namespace CityZero.Gameplay.Traffic
{
    public sealed class TrafficVehicleAgent : MonoBehaviour
    {
        [SerializeField] private VehicleController _vehicle;
        [SerializeField] private TrafficLanePath _lane;
        [SerializeField] private float _pointRadius = 1.2f;
        [SerializeField] private float _throttle = 0.8f;

        private int _index;

        private void Update()
        {
            if (_vehicle == null || _lane == null || _lane.PointCount == 0 || _vehicle.HasDriver)
            {
                return;
            }

            Vector3 target = _lane.GetPoint(_index);
            Vector3 local = transform.InverseTransformPoint(target);
            float steering = Mathf.Clamp(local.x, -1f, 1f);
            _vehicle.SetInput(_throttle, steering);

            if (Vector3.Distance(transform.position, target) <= _pointRadius)
            {
                _index = (_index + 1) % _lane.PointCount;
            }
        }
    }
}
