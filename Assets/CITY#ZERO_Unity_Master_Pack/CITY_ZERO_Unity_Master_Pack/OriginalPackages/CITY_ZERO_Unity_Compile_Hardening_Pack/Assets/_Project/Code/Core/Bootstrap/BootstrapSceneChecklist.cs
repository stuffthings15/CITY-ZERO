using UnityEngine;

namespace CityZero.Core.Bootstrap
{
    public sealed class BootstrapSceneChecklist : MonoBehaviour
    {
        [TextArea(10, 30)]
        [SerializeField]
        private string _notes =
@"Bootstrap scene objects:
- GameBootstrap
- ConfigDatabase
- SaveManager
- MissionManager
- MissionFactory
- HeatSystem
- Optional DebugCommandRegistry

Frontend scene:
- EventSystem
- InputSystemUIInputModule
- Main menu UI

Sandbox scene:
- Player prefab
- HUD canvas
- District streaming roots";
    }
}
