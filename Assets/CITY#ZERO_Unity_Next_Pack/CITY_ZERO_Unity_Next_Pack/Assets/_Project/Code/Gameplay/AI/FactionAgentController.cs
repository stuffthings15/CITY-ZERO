using UnityEngine;
using CityZero.Gameplay.Player;

namespace CityZero.Gameplay.AI
{
    public sealed class FactionAgentController : AIAgentBase
    {
        public enum FactionState
        {
            PatrolTerritory,
            GuardLocation,
            AttackPlayer,
            Fallback,
            HoldChokepoint
        }

        [SerializeField] private Transform[] _territoryPoints;
        [SerializeField] private PlayerController _player;
        [SerializeField] private float _aggroRange = 10f;

        private FactionState _state = FactionState.PatrolTerritory;
        private int _territoryIndex;

        private void Start()
        {
            if (_territoryPoints != null && _territoryPoints.Length > 0)
            {
                SetMoveTarget(_territoryPoints[0].position);
            }
        }

        protected override void TickAgent(float deltaTime)
        {
            switch (_state)
            {
                case FactionState.PatrolTerritory:
                    Patrol(deltaTime);
                    CheckAggro();
                    break;

                case FactionState.GuardLocation:
                    CheckAggro();
                    break;

                case FactionState.AttackPlayer:
                    if (_player != null)
                    {
                        SetMoveTarget(_player.transform.position);
                        MoveTowardsTarget(deltaTime);
                    }
                    else
                    {
                        _state = FactionState.PatrolTerritory;
                    }
                    break;

                case FactionState.Fallback:
                    MoveTowardsTarget(deltaTime);
                    if (!HasMoveTarget)
                    {
                        _state = FactionState.GuardLocation;
                    }
                    break;

                case FactionState.HoldChokepoint:
                    CheckAggro();
                    break;
            }
        }

        private void Patrol(float deltaTime)
        {
            if (_territoryPoints == null || _territoryPoints.Length == 0)
            {
                return;
            }

            MoveTowardsTarget(deltaTime);
            if (!HasMoveTarget)
            {
                _territoryIndex = (_territoryIndex + 1) % _territoryPoints.Length;
                SetMoveTarget(_territoryPoints[_territoryIndex].position);
            }
        }

        private void CheckAggro()
        {
            if (_player == null)
            {
                return;
            }

            if (Vector3.Distance(transform.position, _player.transform.position) <= _aggroRange)
            {
                _state = FactionState.AttackPlayer;
            }
        }

        public string GetStateName() => _state.ToString();
    }
}
