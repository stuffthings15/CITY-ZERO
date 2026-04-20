Demo: Inventory wiring

Purpose
-------
This demo describes how to verify the in-game inventory wiring without disrupting the main project.

Steps
-----
1. Open Unity and load the CITY//ZERO project root.
2. Create a new Scene: Scenes/Demo_Inventory.unity (or open an existing bootstrap scene).
3. Create an empty GameObject named "GameBootstrap" and attach the GameBootstrap MonoBehaviour (Assets/_Project/Code/Core/Bootstrap/GameBootstrap.cs).
   - Ensure the ConfigDatabase field is assigned (or add a placeholder ConfigDatabase ScriptableObject) so Awake() runs successfully.
4. On the same GameObject, either assign a SimpleInventorySystem component in the inspector or leave it empty; GameBootstrap will add it at Awake if missing.
5. Create another empty GameObject named "InventoryDebugger" and attach the InventoryDebugger script (Assets/_Project/Code/Gameplay/Inventory/InventoryDebugger.cs).
6. Run the scene. In Console you should see the inventory add/remove logs from InventoryDebugger.

Notes
-----
- The Inventory uses a pure data class SimpleInventory for logic so it can be unit-tested outside Unity.
- If you use Addressables, mark the bootstrap GameObject as addressable or ensure the scene is included in the build.

Cleanup
-------
- The GameBootstrap GameObject is marked DontDestroyOnLoad so remove it from runtime manually if used only for debugging.
