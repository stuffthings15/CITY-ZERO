namespace CityZero.Gameplay.Combat
{
    public interface IDamageable
    {
        void ApplyDamage(int amount);
        bool IsAlive { get; }
    }
}
