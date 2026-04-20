using Xunit;
using CityZero.Gameplay.Inventory;

namespace SimpleInventory.Tests
{
    public class SimpleInventoryTests
    {
        [Fact]
        public void AddAndRemoveBehavior_WorksAsExpected()
        {
            var inv = new global::CityZero.Gameplay.Inventory.SimpleInventory();

            Assert.Equal(0, inv.GetCount("item1"));

            inv.AddItem("item1", 2);
            Assert.Equal(2, inv.GetCount("item1"));

            var removed = inv.RemoveItem("item1", 1);
            Assert.True(removed);
            Assert.Equal(1, inv.GetCount("item1"));

            removed = inv.RemoveItem("item1", 1);
            Assert.True(removed);
            Assert.Equal(0, inv.GetCount("item1"));

            // cannot remove more than available
            removed = inv.RemoveItem("item1", 1);
            Assert.False(removed);
        }
    }
}
