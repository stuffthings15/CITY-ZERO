using UnityEngine;
using CityZero.Core.EventBus;
using CityZero.Gameplay.Player;

namespace CityZero.Gameplay.Missions
{
    [RequireComponent(typeof(Collider))]
    public sealed class MissionPickupTrigger : MonoBehaviour
    {
        [SerializeField] private string _pickupId;
        [SerializeField] private bool _consumeOnPickup = true;

        private bool _consumed;

        private void OnTriggerEnter(Collider other)
        {
            if (_consumed && _consumeOnPickup)
            {
                return;
            }

            if (!other.TryGetComponent(out PlayerController _))
            {
                return;
            }

            MissionGameplayEvents.RaiseItemPickedUp(_pickupId);
            _consumed = true;
            Debug.Log($"Mission pickup collected: {_pickupId}");
        }
    }
}
