using UnityEngine;

namespace CityZero.UI.Debug
{
    public sealed class SceneReferenceChecklist : MonoBehaviour
    {
        [TextArea(10, 30)]
        [SerializeField]
        private string _notes =
@"Reference checklist:
- ConfigDatabase JSON text assets assigned
- MissionManagerSafe has ConfigDatabase
- MissionFactorySafe has ConfigDatabase, HeatSystem, Player transform
- SaveManager has Player transform
- Player prefab has Rigidbody, PlayerController, PlayerInput, WeaponMotor
- HUD canvas has HudPresenterSafe references
- Vehicle prefab has VehicleControllerClean and VehicleSeat";
    }
}
