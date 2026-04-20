using UnityEngine;

namespace CityZero.Core.SaveLoad
{
    public sealed class SaveManager : MonoBehaviour
    {
        [SerializeField] private string _currentSlotId = "slot_01";
        [SerializeField] private Transform _playerTransform;

        private SaveSystem _saveSystem;

        private void Awake()
        {
            _saveSystem = new SaveSystem();
        }

        public void SaveNow()
        {
            SaveGameData data = Capture();
            _saveSystem.Save(_currentSlotId, data);
            Debug.Log($"Saved game to {_currentSlotId}");
        }

        public void LoadNow()
        {
            SaveGameData data = _saveSystem.Load(_currentSlotId);
            if (data == null)
            {
                Debug.LogWarning("No save found.");
                return;
            }

            Apply(data);
            Debug.Log($"Loaded game from {_currentSlotId}");
        }

        private SaveGameData Capture()
        {
            var data = new SaveGameData();
            if (_playerTransform != null)
            {
                Vector3 pos = _playerTransform.position;
                data.playerPosition = new SerializableVector3(pos.x, pos.y, pos.z);
            }

            return data;
        }

        private void Apply(SaveGameData data)
        {
            if (_playerTransform != null)
            {
                _playerTransform.position = new Vector3(data.playerPosition.x, data.playerPosition.y, data.playerPosition.z);
            }
        }
    }
}
