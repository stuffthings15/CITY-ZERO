using Xunit;
using CityZero.Gameplay.Heat;

namespace HeatModel.Tests
{
    public class HeatModelTests
    {
        [Fact]
        public void HeatDecaysOverTime()
        {
            var model = new global::CityZero.Gameplay.Heat.HeatModel(decayPerSecond: 10f);
            model.CommitCrime(5, 0, 1f); // adds some heat
            float initial = model.HeatScore;
            model.UpdateDecay(0.5f);
            Assert.True(model.HeatScore < initial);
        }

        [Fact]
        public void HeatLevelIncreasesAfterCrime()
        {
            var model = new global::CityZero.Gameplay.Heat.HeatModel(decayPerSecond: 0f);
            Assert.Equal(0, model.HeatLevel);
            model.CommitCrime(50, 0, 1f);
            // A severity of 50 maps into heat score 50 which corresponds to heat level 3 (threshold <70).
            Assert.True(model.HeatLevel >= 3);
        }
    }
}
