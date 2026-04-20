using UnityEngine;
using CityZero.Core.EventBus;
using CityZero.Gameplay.Player;

namespace CityZero.Gameplay.Missions
{
    [RequireComponent(typeof(Collider))]
    public sealed class MissionDeliveryTrigger : MonoBehaviour
    {
        [SerializeField] private string _deliveryId;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController _))
            {
                return;
            }

            MissionGameplayEvents.RaiseItemDelivered(_deliveryId);
            Debug.Log($"Mission delivery completed: {_deliveryId}");
        }
    }
}
