using UnityEngine;
using CityZero.Gameplay.Heat;

namespace CityZero.Gameplay.AI
{
    public sealed class CivilianAIController : AIAgentBase
    {
        public enum CivilianState
        {
            Idle,
            Wander,
            Panic,
            Flee,
            ReportCrime,
            Cower,
            ReturnToRoutine
        }

        [Header("Wander")]
        [SerializeField] private float _wanderRadius = 5f;
        [SerializeField] private float _wanderInterval = 3f;

        [Header("Threat")]
        [SerializeField] private Transform _threatSource;
        [SerializeField] private float _panicDistance = 8f;
        [SerializeField] private float _reportRange = 12f;
        [SerializeField] private HeatSystem _heatSystem;

        private CivilianState _state = CivilianState.Wander;
        private Vector3 _homePosition;
        private float _nextWanderTime;

        private void Start()
        {
            _homePosition = transform.position;
            PickWanderPoint();
        }

        protected override void TickAgent(float deltaTime)
        {
            switch (_state)
            {
                case CivilianState.Idle:
                    if (Time.time >= _nextWanderTime)
                    {
                        _state = CivilianState.Wander;
                        PickWanderPoint();
                    }
                    break;

                case CivilianState.Wander:
                    MoveTowardsTarget(deltaTime);
                    if (!HasMoveTarget)
                    {
                        _state = CivilianState.Idle;
                        _nextWanderTime = Time.time + _wanderInterval;
                    }
                    break;

                case CivilianState.Panic:
                    if (_threatSource != null)
                    {
                        Vector3 away = (transform.position - _threatSource.position).normalized;
                        SetMoveTarget(transform.position + away * _panicDistance);
                        _state = CivilianState.Flee;
                    }
                    else
                    {
                        _state = CivilianState.Cower;
                    }
                    break;

                case CivilianState.Flee:
                    MoveTowardsTarget(deltaTime);
                    if (!HasMoveTarget)
                    {
                        _state = CivilianState.ReportCrime;
                    }
                    break;

                case CivilianState.ReportCrime:
                    TryReportCrime();
                    _state = CivilianState.ReturnToRoutine;
                    SetMoveTarget(_homePosition);
                    break;

                case CivilianState.Cower:
                    break;

                case CivilianState.ReturnToRoutine:
                    MoveTowardsTarget(deltaTime);
                    if (!HasMoveTarget)
                    {
                        _state = CivilianState.Idle;
                        _nextWanderTime = Time.time + _wanderInterval;
                    }
                    break;
            }
        }

        public void TriggerPanic(Transform threatSource)
        {
            _threatSource = threatSource;
            _state = CivilianState.Panic;
        }

        private void PickWanderPoint()
        {
            Vector2 random = Random.insideUnitCircle * _wanderRadius;
            SetMoveTarget(_homePosition + new Vector3(random.x, 0f, random.y));
        }

        private void TryReportCrime()
        {
            if (_heatSystem != null && _threatSource != null && Vector3.Distance(transform.position, _threatSource.position) <= _reportRange * 2f)
            {
                _heatSystem.CommitCrime(severity: 2, witnessCount: 1, districtMultiplier: 1f);
            }
        }

        public string GetStateName() => _state.ToString();
    }
}
