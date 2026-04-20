using UnityEngine;
using CityZero.Core.EventBus;

namespace CityZero.Gameplay.AI
{
    // Very simple police AI that listens for heat level changes and logs a response.
    public class SimplePoliceAI : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEventBus.HeatLevelChanged += OnHeatLevelChanged;
        }

        private void OnDisable()
        {
            GameEventBus.HeatLevelChanged -= OnHeatLevelChanged;
        }

        private void OnHeatLevelChanged(CityZero.Gameplay.Heat.HeatLevelChangedEvent evt)
        {
            Debug.Log($"[SimplePoliceAI] Heat changed from {evt.PreviousLevel} to {evt.CurrentLevel}");
            if (evt.CurrentLevel >= 2)
            {
                // For now, just log intent to dispatch units
                Debug.Log("[SimplePoliceAI] Dispatching patrol units to investigate.");
            }
        }
    }
}
