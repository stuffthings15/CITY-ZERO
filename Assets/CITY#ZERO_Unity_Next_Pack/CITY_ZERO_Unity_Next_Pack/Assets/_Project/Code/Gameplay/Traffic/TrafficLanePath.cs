using UnityEngine;

namespace CityZero.Gameplay.Traffic
{
    public sealed class TrafficLanePath : MonoBehaviour
    {
        [SerializeField] private Transform[] _points;

        public int PointCount => _points != null ? _points.Length : 0;

        public Vector3 GetPoint(int index)
        {
            if (_points == null || _points.Length == 0)
            {
                return transform.position;
            }

            index = Mathf.Clamp(index, 0, _points.Length - 1);
            return _points[index].position;
        }
    }
}
