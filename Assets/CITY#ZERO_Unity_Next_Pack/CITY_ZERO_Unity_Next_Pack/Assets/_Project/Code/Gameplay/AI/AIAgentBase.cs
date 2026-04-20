using UnityEngine;

namespace CityZero.Gameplay.AI
{
    public abstract class AIAgentBase : MonoBehaviour
    {
        [SerializeField] protected float _moveSpeed = 3.5f;
        [SerializeField] protected float _rotationSpeed = 10f;
        [SerializeField] protected float _stoppingDistance = 0.2f;

        protected Vector3 _moveTarget;
        protected bool _hasMoveTarget;

        protected virtual void Update()
        {
            TickAgent(Time.deltaTime);
        }

        protected abstract void TickAgent(float deltaTime);

        protected void MoveTowardsTarget(float deltaTime)
        {
            if (!_hasMoveTarget)
            {
                return;
            }

            Vector3 toTarget = _moveTarget - transform.position;
            toTarget.y = 0f;
            float distance = toTarget.magnitude;

            if (distance <= _stoppingDistance)
            {
                _hasMoveTarget = false;
                return;
            }

            Vector3 direction = toTarget.normalized;
            transform.position += direction * (_moveSpeed * deltaTime);

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * deltaTime);
            }
        }

        public virtual void SetMoveTarget(Vector3 worldPosition)
        {
            _moveTarget = worldPosition;
            _hasMoveTarget = true;
        }

        public bool HasMoveTarget => _hasMoveTarget;
        public Vector3 MoveTarget => _moveTarget;
    }
}
