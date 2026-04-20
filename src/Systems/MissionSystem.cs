using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using CityZero.Core;
using CityZero.Core.Events;
using CityZero.Data.Definitions;

namespace CityZero.Systems
{
    /// <summary>
    /// Manages all active missions: lifecycle, objective tracking, and reward delivery.
    /// Missions are data-driven; all definitions loaded from JSON via DataRegistry.
    /// Max 3 concurrent active missions.
    /// </summary>
    public partial class MissionSystem : Node
    {
        private const int MAX_ACTIVE_MISSIONS = 3;

        private readonly Dictionary<string, MissionInstance> _activeMissions = new();
        private readonly HashSet<string> _completedMissions = new();
        private readonly HashSet<string> _failedMissions = new();

        public override void _Ready()
        {
            ServiceLocator.Register<MissionSystem>(this);
            GameBus.Instance.Subscribe<PlayerDiedEvent>(OnPlayerDied);
            GD.Print("[MissionSystem] Ready.");
        }

        public override void _ExitTree()
        {
            GameBus.Instance.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
        }

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>Attempts to activate a mission by ID. Returns false if already active, at cap, or unavailable.</summary>
        public bool ActivateMission(string missionId)
        {
            if (_activeMissions.ContainsKey(missionId))
            {
                GD.PrintErr($"[MissionSystem] Mission {missionId} is already active.");
                return false;
            }
            if (_activeMissions.Count >= MAX_ACTIVE_MISSIONS)
            {
                GD.Print($"[MissionSystem] Cannot activate {missionId}: max active mission count reached.");
                return false;
            }

            // TODO: Load MissionDefinition from DataRegistry when implemented
            var instance = new MissionInstance(missionId);
            _activeMissions[missionId] = instance;
            instance.OnObjectiveCompleted += HandleObjectiveCompleted;
            instance.OnMissionResolved += HandleMissionResolved;

            GameBus.Instance.Emit(new MissionStateChangedEvent(
                missionId, MissionState.Available, MissionState.Active));

            GD.Print($"[MissionSystem] Mission activated: {missionId}");
            return true;
        }

        /// <summary>Reports a game event to all active missions for objective evaluation.</summary>
        public void ReportEvent(string eventType, string targetId, object payload = null)
        {
            foreach (var mission in _activeMissions.Values.ToList())
                mission.EvaluateEvent(eventType, targetId, payload);
        }

        public bool IsMissionCompleted(string missionId) => _completedMissions.Contains(missionId);
        public bool IsMissionFailed(string missionId)    => _failedMissions.Contains(missionId);
        public bool IsMissionActive(string missionId)    => _activeMissions.ContainsKey(missionId);
        public int  ActiveCount                          => _activeMissions.Count;

        /// <summary>Returns the live MissionInstance (for tests and objective reporting).</summary>
        public bool TryGetActive(string missionId, out MissionInstance instance)
            => _activeMissions.TryGetValue(missionId, out instance);

        // ── Event Handlers ────────────────────────────────────────────────────────

        private void OnPlayerDied(PlayerDiedEvent e)
        {
            foreach (var id in _activeMissions.Keys.ToList())
                FailMission(id, "player_death");
        }

        private void HandleObjectiveCompleted(string missionId, string objectiveId, bool isOptional)
        {
            GameBus.Instance.Emit(new ObjectiveCompletedEvent(missionId, objectiveId, isOptional));
            GD.Print($"[MissionSystem] Objective '{objectiveId}' completed in '{missionId}' (optional: {isOptional})");
        }

        private void HandleMissionResolved(string missionId, bool success)
        {
            if (success) CompleteMission(missionId);
            else FailMission(missionId, "objective_failed");
        }

        // ── Private ───────────────────────────────────────────────────────────────

        private void CompleteMission(string missionId)
        {
            if (!_activeMissions.Remove(missionId, out var instance)) return;
            _completedMissions.Add(missionId);

            // TODO: Deliver rewards from MissionDefinition (cash, rep, unlock flags)
            GameBus.Instance.Emit(new MissionStateChangedEvent(
                missionId, MissionState.Active, MissionState.Completed));

            GD.Print($"[MissionSystem] Mission COMPLETED: {missionId}");
        }

        private void FailMission(string missionId, string reason)
        {
            if (!_activeMissions.Remove(missionId, out var instance)) return;
            _failedMissions.Add(missionId);

            GameBus.Instance.Emit(new MissionStateChangedEvent(
                missionId, MissionState.Active, MissionState.Failed));

            GD.Print($"[MissionSystem] Mission FAILED: {missionId} | Reason: {reason}");
        }
    }

    /// <summary>Tracks runtime state of a single active mission.</summary>
    public class MissionInstance
    {
        public string MissionId { get; }

        public event Action<string, string, bool> OnObjectiveCompleted;
        public event Action<string, bool> OnMissionResolved;

        // TODO: Populate from MissionDefinition JSON
        private readonly List<string> _requiredObjectives = new();
        private readonly HashSet<string> _completedObjectives = new();

        public MissionInstance(string missionId)
        {
            MissionId = missionId;
        }

        /// <summary>
        /// Registers a required objective. Call this when populating from a MissionDefinition.
        /// </summary>
        public void AddObjective(string objectiveId, bool isOptional = false)
        {
            if (!isOptional)
                _requiredObjectives.Add(objectiveId);
            _optionalObjectives[objectiveId] = isOptional;
        }

        /// <summary>
        /// Marks an objective as complete. Fires events and resolves the mission if all required
        /// objectives are now done.
        /// </summary>
        public void CompleteObjective(string objectiveId)
        {
            if (_completedObjectives.Contains(objectiveId)) return;

            bool isOptional = _optionalObjectives.TryGetValue(objectiveId, out var opt) && opt;
            _completedObjectives.Add(objectiveId);
            OnObjectiveCompleted?.Invoke(MissionId, objectiveId, isOptional);
            CheckAllRequired();
        }

        /// <summary>Evaluates an incoming game event against all pending objectives.</summary>
        public void EvaluateEvent(string eventType, string targetId, object payload)
        {
            // TODO: Match against objective definitions loaded from MissionDefinition.
            // For now, if the eventType == objectiveId, treat it as completion.
            CompleteObjective(eventType);
        }

        // ── Private ───────────────────────────────────────────────────────────────

        private readonly Dictionary<string, bool> _optionalObjectives = new();

        private void CheckAllRequired()
        {
            bool allDone = _requiredObjectives.Count > 0 &&
                           _requiredObjectives.All(o => _completedObjectives.Contains(o));
            if (allDone)
                OnMissionResolved?.Invoke(MissionId, true);
        }
    }
}
