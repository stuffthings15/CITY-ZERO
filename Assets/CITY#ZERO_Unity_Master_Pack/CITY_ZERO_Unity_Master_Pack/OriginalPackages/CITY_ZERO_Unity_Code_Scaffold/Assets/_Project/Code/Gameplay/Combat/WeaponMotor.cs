using UnityEngine;

namespace CityZero.Gameplay.Combat
{
    public sealed class WeaponMotor : MonoBehaviour, IWeaponMotor
    {
        [SerializeField] private Transform _firePoint;
        [SerializeField] private float _fireRate = 8f;
        [SerializeField] private float _range = 25f;
        [SerializeField] private int _damage = 10;
        [SerializeField] private LayerMask _hitMask = ~0;

        private bool _fireHeld;
        private float _nextFireTime;

        private void Update()
        {
            if (_fireHeld && Time.time >= _nextFireTime)
            {
                FireShot();
            }
        }

        public void FirePressed()
        {
            _fireHeld = true;
            if (Time.time >= _nextFireTime)
            {
                FireShot();
            }
        }

        public void FireReleased()
        {
            _fireHeld = false;
        }

        public void Reload()
        {
            Debug.Log("Reload requested.");
        }

        private void FireShot()
        {
            _nextFireTime = Time.time + (1f / Mathf.Max(0.01f, _fireRate));

            Vector3 origin = _firePoint != null ? _firePoint.position : transform.position;
            Vector3 direction = _firePoint != null ? _firePoint.right : transform.right;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, _range, _hitMask))
            {
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.ApplyDamage(_damage);
                }
            }
        }
    }
}
