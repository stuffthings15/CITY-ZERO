using UnityEngine;

namespace CityZero.UI.Debug
{
    public sealed class InputActionsDebugNote : MonoBehaviour
    {
        [TextArea(6, 20)]
        [SerializeField]
        private string _note =
@"Bind the CITY_ZERO input actions asset to your PlayerInput component or generated wrapper.
Check action names:
- Player/Move
- Player/Look
- Player/Fire
- Vehicle/Move
- UI/Navigate";
    }
}
