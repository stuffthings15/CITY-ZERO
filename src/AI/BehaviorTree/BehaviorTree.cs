using Godot;
using System;
using System.Collections.Generic;
using CityZero.Core;

namespace CityZero.AI
{
    /// <summary>
    /// Core Behavior Tree node types. All AI behavior is composed from these building blocks.
    /// Trees are built in code or loaded from data and ticked each AI frame.
    /// </summary>
    public enum BTStatus { Success, Failure, Running }

    public abstract class BTNode
    {
        public abstract BTStatus Tick(AIBlackboard board);
        public virtual void Reset(AIBlackboard board) { }
    }

    /// <summary>Runs children in order; returns Success when first child succeeds (OR logic).</summary>
    public class BTSelector : BTNode
    {
        private readonly List<BTNode> _children;
        private int _runningIndex = -1;

        public BTSelector(params BTNode[] children) => _children = new List<BTNode>(children);

        public override BTStatus Tick(AIBlackboard board)
        {
            int start = _runningIndex >= 0 ? _runningIndex : 0;
            for (int i = start; i < _children.Count; i++)
            {
                var status = _children[i].Tick(board);
                if (status == BTStatus.Running)  { _runningIndex = i; return BTStatus.Running; }
                if (status == BTStatus.Success)  { _runningIndex = -1; return BTStatus.Success; }
            }
            _runningIndex = -1;
            return BTStatus.Failure;
        }

        public override void Reset(AIBlackboard board)
        {
            _runningIndex = -1;
            foreach (var child in _children) child.Reset(board);
        }
    }

    /// <summary>Runs children in order; returns Failure if any child fails (AND logic).</summary>
    public class BTSequence : BTNode
    {
        private readonly List<BTNode> _children;
        private int _runningIndex = -1;

        public BTSequence(params BTNode[] children) => _children = new List<BTNode>(children);

        public override BTStatus Tick(AIBlackboard board)
        {
            int start = _runningIndex >= 0 ? _runningIndex : 0;
            for (int i = start; i < _children.Count; i++)
            {
                var status = _children[i].Tick(board);
                if (status == BTStatus.Running)  { _runningIndex = i; return BTStatus.Running; }
                if (status == BTStatus.Failure)  { _runningIndex = -1; return BTStatus.Failure; }
            }
            _runningIndex = -1;
            return BTStatus.Success;
        }

        public override void Reset(AIBlackboard board)
        {
            _runningIndex = -1;
            foreach (var child in _children) child.Reset(board);
        }
    }

    /// <summary>Inverts child result: Success → Failure; Failure → Success; Running unchanged.</summary>
    public class BTInverter : BTNode
    {
        private readonly BTNode _child;
        public BTInverter(BTNode child) => _child = child;

        public override BTStatus Tick(AIBlackboard board)
        {
            var status = _child.Tick(board);
            return status switch
            {
                BTStatus.Success => BTStatus.Failure,
                BTStatus.Failure => BTStatus.Success,
                _                => BTStatus.Running,
            };
        }
    }

    /// <summary>Condition node — wraps a predicate function. Instant Success or Failure.</summary>
    public class BTCondition : BTNode
    {
        private readonly Func<AIBlackboard, bool> _predicate;
        private readonly string _debugLabel;

        public BTCondition(Func<AIBlackboard, bool> predicate, string debugLabel = "")
        {
            _predicate = predicate;
            _debugLabel = debugLabel;
        }

        public override BTStatus Tick(AIBlackboard board)
            => _predicate(board) ? BTStatus.Success : BTStatus.Failure;
    }

    /// <summary>Action node — wraps a stateful action function. Returns Running until complete.</summary>
    public class BTAction : BTNode
    {
        private readonly Func<AIBlackboard, BTStatus> _action;
        private readonly string _debugLabel;

        public BTAction(Func<AIBlackboard, BTStatus> action, string debugLabel = "")
        {
            _action = action;
            _debugLabel = debugLabel;
        }

        public override BTStatus Tick(AIBlackboard board) => _action(board);
    }

    /// <summary>
    /// Per-NPC data context. Shared between all nodes in a BT during a single tick.
    /// Typed key-value store — all keys are string constants defined per agent type.
    /// </summary>
    public class AIBlackboard
    {
        private readonly Dictionary<string, object> _data = new();

        public void Set<T>(string key, T value) => _data[key] = value;

        public T Get<T>(string key, T defaultValue = default)
        {
            if (_data.TryGetValue(key, out var val) && val is T typed)
                return typed;
            return defaultValue;
        }

        public bool Has(string key) => _data.ContainsKey(key);
        public void Remove(string key) => _data.Remove(key);
        public void Clear() => _data.Clear();
    }

    // ── Blackboard Key Constants ─────────────────────────────────────────────────
    public static class BB
    {
        // Agent self-reference
        public const string Self               = "self";
        public const string IsUnderAttack      = "is_under_attack";
        public const string LastAttacker       = "last_attacker";
        public const string CurrentCover       = "current_cover";
        public const string PatrolWaypointIdx  = "patrol_waypoint_idx";
        public const string InvestigateTarget  = "investigate_target";
        public const string FleeTarget         = "flee_target";
        public const string IsPlayerHostile    = "is_player_hostile";
        public const string HealthNormalized   = "health_normalized";
        public const string CanSeePlayer       = "can_see_player";
        public const string AlertLevel         = "alert_level";   // 0=idle, 1=aware, 2=alarmed, 3=combat
    }
}
