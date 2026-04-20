using UnityEditor;
using UnityEngine;

namespace CityZero.Editor.MenuItems
{
    public static class EditorWorkflowShortcuts
    {
        [MenuItem("CityZero/Workflow/Print Setup Checklist")]
        public static void PrintSetupChecklist()
        {
            Debug.Log(
@"CITY//ZERO setup checklist:
1. Import base scaffold
2. Import next pack
3. Import compile-hardening pack
4. Import scene and cleanup pack
5. Import vertical slice wiring pack
6. Import editor workflow pack
7. Generate or assign config JSON
8. Create bootstrap, sandbox, and district scenes
9. Create player and base vehicle prefabs
10. Run scene validation");
        }
    }
}
