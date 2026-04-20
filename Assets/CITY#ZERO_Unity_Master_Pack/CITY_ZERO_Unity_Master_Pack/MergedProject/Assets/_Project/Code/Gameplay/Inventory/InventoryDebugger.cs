using UnityEngine;

namespace CityZero.Gameplay.Inventory
{
    // Simple runtime debug component to exercise the inventory at Start
    public class InventoryDebugger : MonoBehaviour
    {
        [SerializeField]
        private string testItemId = "health_pack";

        private void Start()
        {
            var inv = CityZero.Core.Bootstrap.GameBootstrap.Inventory;
            if (inv == null)
            {
                Debug.LogError("Inventory not initialized. Ensure GameBootstrap is present in the bootstrap scene and Inventory is assigned.");
                return;
            }

            Debug.Log($"[InventoryDebugger] Initial count for '{testItemId}': {inv.GetCount(testItemId)}");

            inv.AddItem(testItemId, 3);
            Debug.Log($"[InventoryDebugger] After Add(3): {inv.GetCount(testItemId)}");

            var removed = inv.RemoveItem(testItemId, 2);
            Debug.Log($"[InventoryDebugger] Removed 2 -> success={removed}; count={inv.GetCount(testItemId)}");

            // Try removing more than available
            removed = inv.RemoveItem(testItemId, 5);
            Debug.Log($"[InventoryDebugger] Removed 5 -> success={removed}; final count={inv.GetCount(testItemId)}");
        }
    }
}
