namespace CityZero.Gameplay.Player
{
    public interface IInteractable
    {
        void Interact(PlayerController player);
        string GetInteractionLabel();
    }
}
