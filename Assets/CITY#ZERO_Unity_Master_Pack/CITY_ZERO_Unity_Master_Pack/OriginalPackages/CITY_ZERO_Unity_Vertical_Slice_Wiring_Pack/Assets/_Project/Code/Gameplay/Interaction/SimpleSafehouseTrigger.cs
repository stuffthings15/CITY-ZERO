using UnityEngine;
using CityZero.Core.SaveLoad;
using CityZero.Gameplay.Player;

namespace CityZero.Gameplay.Interaction
{
    [RequireComponent(typeof(Collider))]
    public sealed class SimpleSafehouseTrigger : MonoBehaviour
    {
        [SerializeField] private SaveManager _saveManager;
        [SerializeField] private bool _saveOnEnter = true;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController _))
            {
                return;
            }

            if (_saveOnEnter)
            {
                _saveManager?.SaveNow();
            }
        }
    }
}
