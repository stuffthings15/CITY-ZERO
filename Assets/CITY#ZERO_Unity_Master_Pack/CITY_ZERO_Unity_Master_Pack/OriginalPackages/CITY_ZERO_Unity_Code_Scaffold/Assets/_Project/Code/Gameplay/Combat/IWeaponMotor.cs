namespace CityZero.Gameplay.Combat
{
    public interface IWeaponMotor
    {
        void FirePressed();
        void FireReleased();
        void Reload();
    }
}
