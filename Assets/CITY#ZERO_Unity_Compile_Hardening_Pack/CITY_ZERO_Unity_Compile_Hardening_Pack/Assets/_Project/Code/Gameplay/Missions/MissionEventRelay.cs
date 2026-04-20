using System.Collections.Generic;
using CityZero.Core.EventBus;

namespace CityZero.Gameplay.Missions
{
    public sealed class MissionEventRelay
    {
        private readonly List<IMissionObjectiveHandler> _handlers;

        public MissionEventRelay(List<IMissionObjectiveHandler> handlers)
        {
            _handlers = handlers;
            MissionGameplayEvents.ItemPickedUp += OnItemPickedUp;
            MissionGameplayEvents.ItemDelivered += OnItemDelivered;
            MissionGameplayEvents.TargetDestroyed += OnTargetDestroyed;
        }

        public void Dispose()
        {
            MissionGameplayEvents.ItemPickedUp -= OnItemPickedUp;
            MissionGameplayEvents.ItemDelivered -= OnItemDelivered;
            MissionGameplayEvents.TargetDestroyed -= OnTargetDestroyed;
        }

        private void OnItemPickedUp(string id)
        {
            foreach (var handler in _handlers)
            {
                if (handler is PickupObjectiveHandler pickup)
                {
                    pickup.NotifyPickedUp(id);
                }
            }
        }

        private void OnItemDelivered(string id)
        {
            foreach (var handler in _handlers)
            {
                if (handler is DeliverObjectiveHandler deliver)
                {
                    deliver.NotifyDelivered(id);
                }
            }
        }

        private void OnTargetDestroyed(string id)
        {
            foreach (var handler in _handlers)
            {
                if (handler is DestroyTargetObjectiveHandler destroy)
                {
                    destroy.NotifyDestroyed(id);
                }
            }
        }
    }
}
