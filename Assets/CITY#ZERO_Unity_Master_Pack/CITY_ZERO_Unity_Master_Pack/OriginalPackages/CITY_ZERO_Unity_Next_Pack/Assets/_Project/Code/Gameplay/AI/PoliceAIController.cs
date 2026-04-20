using UnityEngine;
using CityZero.Gameplay.Player;

namespace CityZero.Gameplay.AI
{
    public sealed class PoliceAIController : AIAgentBase
    {
        public enum PoliceState
        {
            Patrol,
            Investigate,
            PursueOnFoot,
            CombatContain,
            ArrestAttempt,
            SearchLastKnown,
            ReturnPatrol
        }

        [SerializeField] private Transform[] _patrolPoints;
        [SerializeField] private float _visionRange = 12f;
        [SerializeField] private float _arrestDistance = 1.5f;
        [SerializeField] private float _searchDuration = 6f;
        [SerializeField] private PlayerController _player;

        private PoliceState _state = PoliceState.Patrol;
        private int _patrolIndex;
        private float _searchEndTime;
        private Vector3 _lastKnownPlayerPosition;

        private void Start()
        {
            if (_patrolPoints != null && _patrolPoints.Length > 0)
            {
                SetMoveTarget(_patrolPoints[0].position);
            }
        }

        protected override void TickAgent(float deltaTime)
        {
            switch (_state)
            {
                case PoliceState.Patrol:
                    Patrol(deltaTime);
                    ScanForPlayer();
                    break;

                case PoliceState.Investigate:
                    MoveTowardsTarget(deltaTime);
                    ScanForPlayer();
                    if (!HasMoveTarget)
                    {
                        _state = PoliceState.SearchLastKnown;
                        _searchEndTime = Time.time + _searchDuration;
                    }
                    break;

                case PoliceState.PursueOnFoot:
                    if (_player == null)
                    {
                        _state = PoliceState.ReturnPatrol;
                        break;
                    }

                    _lastKnownPlayerPosition = _player.transform.position;
                    SetMoveTarget(_lastKnownPlayerPosition);
                    MoveTowardsTarget(deltaTime);

                    if (Vector3.Distance(transform.position, _player.transform.position) <= _arrestDistance)
                    {
                        _state = PoliceState.ArrestAttempt;
                    }
                    break;

                case PoliceState.ArrestAttempt:
                    if (_player != null)
                    {
                        SetMoveTarget(_player.transform.position);
                    }
                    _state = PoliceState.SearchLastKnown;
                    _searchEndTime = Time.time + _searchDuration;
                    break;

                case PoliceState.SearchLastKnown:
                    SetMoveTarget(_lastKnownPlayerPosition);
                    MoveTowardsTarget(deltaTime);
                    if (Time.time >= _searchEndTime)
                    {
                        _state = PoliceState.ReturnPatrol;
                    }
                    break;

                case PoliceState.ReturnPatrol:
                    if (_patrolPoints != null && _patrolPoints.Length > 0)
                    {
                        SetMoveTarget(_patrolPoints[_patrolIndex].position);
                        _state = PoliceState.Patrol;
                    }
                    break;

                case PoliceState.CombatContain:
                    MoveTowardsTarget(deltaTime);
                    break;
            }
        }

        public void InvestigatePoint(Vector3 worldPosition)
        {
            _lastKnownPlayerPosition = worldPosition;
            SetMoveTarget(worldPosition);
            _state = PoliceState.Investigate;
        }

        private void Patrol(float deltaTime)
        {
            if (_patrolPoints == null || _patrolPoints.Length == 0)
            {
                return;
            }

            MoveTowardsTarget(deltaTime);

            if (!HasMoveTarget)
            {
                _patrolIndex = (_patrolIndex + 1) % _patrolPoints.Length;
                SetMoveTarget(_patrolPoints[_patrolIndex].position);
            }
        }

        private void ScanForPlayer()
        {
            if (_player == null)
            {
                return;
            }

            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if (distance <= _visionRange)
            {
                _state = PoliceState.PursueOnFoot;
            }
        }

        public string GetStateName() => _state.ToString();
    }
}
