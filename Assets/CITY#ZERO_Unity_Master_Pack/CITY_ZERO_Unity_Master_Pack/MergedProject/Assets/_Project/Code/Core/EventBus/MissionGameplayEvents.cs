using System;

namespace CityZero.Core.EventBus
{
    public static class MissionGameplayEvents
    {
        public static event Action<string> ItemPickedUp;
        public static event Action<string> ItemDelivered;
        public static event Action<string> TargetDestroyed;
        public static event Action<string> ZoneEntered;
        public static event Action<string> ZoneExited;

        public static void RaiseItemPickedUp(string id) => ItemPickedUp?.Invoke(id);
        public static void RaiseItemDelivered(string id) => ItemDelivered?.Invoke(id);
        public static void RaiseTargetDestroyed(string id) => TargetDestroyed?.Invoke(id);
        public static void RaiseZoneEntered(string id) => ZoneEntered?.Invoke(id);
        public static void RaiseZoneExited(string id) => ZoneExited?.Invoke(id);
    }
}
