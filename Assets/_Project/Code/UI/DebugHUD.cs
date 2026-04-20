using UnityEngine;

namespace CityZero.UI.Debug
{
    public class DebugHUD : MonoBehaviour
    {
        private Heat.HeatSystem _heat;

        private void Start()
        {
            _heat = FindObjectOfType<Heat.HeatSystem>();
        }

        private void OnGUI()
        {
            if (_heat != null)
            {
                GUI.Label(new Rect(10, 10, 300, 20), $"Heat: {_heat.HeatScore:F1} (Level {_heat.HeatLevel})");
            }

            var inv = CityZero.Core.Bootstrap.GameBootstrap.Inventory;
            if (inv != null)
            {
                GUI.Label(new Rect(10, 30, 300, 20), $"Inventory sample count for 'health_pack': {inv.GetCount("health_pack")}");
            }
        }
    }
}
