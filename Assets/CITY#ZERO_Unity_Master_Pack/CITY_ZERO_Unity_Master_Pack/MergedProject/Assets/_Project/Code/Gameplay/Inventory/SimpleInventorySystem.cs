using UnityEngine;

namespace CityZero.Gameplay.Inventory
{
    // Lightweight MonoBehaviour wrapper that delegates to a pure data class for logic and testability
    public sealed class SimpleInventorySystem : MonoBehaviour
    {
        private readonly SimpleInventory _inventory = new();

        public void AddItem(string itemId, int amount = 1) => _inventory.AddItem(itemId, amount);

        public bool RemoveItem(string itemId, int amount = 1) => _inventory.RemoveItem(itemId, amount);

        public int GetCount(string itemId) => _inventory.GetCount(itemId);
    }
}
