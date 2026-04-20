using UnityEngine;

namespace CityZero.Gameplay.Combat
{
    public sealed class HealthComponent : MonoBehaviour, IDamageable
    {
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _currentHealth = 100;

        public bool IsAlive => _currentHealth > 0;
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        private void Awake()
        {
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        }

        public void ApplyDamage(int amount)
        {
            if (!IsAlive || amount <= 0)
            {
                return;
            }

            _currentHealth = Mathf.Max(0, _currentHealth - amount);

            if (_currentHealth == 0)
            {
                Debug.Log($"{name} died.");
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || !IsAlive)
            {
                return;
            }

            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        }
    }
}
