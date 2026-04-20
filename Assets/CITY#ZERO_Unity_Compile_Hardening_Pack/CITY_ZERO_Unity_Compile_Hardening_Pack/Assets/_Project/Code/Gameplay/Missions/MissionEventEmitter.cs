using UnityEngine;
using CityZero.Core.EventBus;

namespace CityZero.Gameplay.Missions
{
    public sealed class MissionEventEmitter : MonoBehaviour
    {
        [SerializeField] private string _eventId;

        [ContextMenu("Emit Pickup")]
        public void EmitPickup() => MissionGameplayEvents.RaiseItemPickedUp(_eventId);

        [ContextMenu("Emit Delivery")]
        public void EmitDelivery() => MissionGameplayEvents.RaiseItemDelivered(_eventId);

        [ContextMenu("Emit Destroyed")]
        public void EmitDestroyed() => MissionGameplayEvents.RaiseTargetDestroyed(_eventId);
    }
}
