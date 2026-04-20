using System.Collections.Generic;

namespace CityZero.Gameplay.Inventory
{
    // Pure data-oriented inventory class (no Unity dependency) to allow unit testing
    public sealed class SimpleInventory
    {
        private readonly Dictionary<string, int> _items = new();

        public void AddItem(string itemId, int amount = 1)
        {
            if (string.IsNullOrWhiteSpace(itemId) || amount <= 0)
            {
                return;
            }

            _items.TryGetValue(itemId, out int current);
            _items[itemId] = current + amount;
        }

        public bool RemoveItem(string itemId, int amount = 1)
        {
            if (!_items.TryGetValue(itemId, out int current) || current < amount)
            {
                return false;
            }

            current -= amount;
            if (current <= 0)
            {
                _items.Remove(itemId);
            }
            else
            {
                _items[itemId] = current;
            }

            return true;
        }

        public int GetCount(string itemId)
        {
            return _items.TryGetValue(itemId, out int count) ? count : 0;
        }
    }
}
