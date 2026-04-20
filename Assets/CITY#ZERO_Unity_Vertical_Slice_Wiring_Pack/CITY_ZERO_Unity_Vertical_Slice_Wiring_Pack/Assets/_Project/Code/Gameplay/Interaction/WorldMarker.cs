using UnityEngine;

namespace CityZero.Gameplay.Interaction
{
    public sealed class WorldMarker : MonoBehaviour
    {
        [SerializeField] private string _markerId;
        [SerializeField] private string _displayName;
        [SerializeField] private MarkerType _markerType;

        public string MarkerId => _markerId;
        public string DisplayName => _displayName;
        public MarkerType Type => _markerType;
    }

    public enum MarkerType
    {
        None = 0,
        Safehouse = 1,
        Garage = 2,
        Shop = 3,
        Mission = 4
    }
}
