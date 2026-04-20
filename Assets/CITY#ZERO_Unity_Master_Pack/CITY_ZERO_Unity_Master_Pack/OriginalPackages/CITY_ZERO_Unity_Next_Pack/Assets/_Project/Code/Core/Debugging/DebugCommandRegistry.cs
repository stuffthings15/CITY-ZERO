using System;
using System.Collections.Generic;
using UnityEngine;
using CityZero.Core.SaveLoad;
using CityZero.Gameplay.Heat;

namespace CityZero.Core.Debugging
{
    public sealed class DebugCommandRegistry : MonoBehaviour
    {
        [SerializeField] private HeatSystem _heatSystem;
        [SerializeField] private SaveManager _saveManager;

        private readonly Dictionary<string, Action<string[]>> _commands = new();

        private void Awake()
        {
            _commands["set_heat"] = SetHeat;
            _commands["save"] = _ => _saveManager?.SaveNow();
            _commands["load"] = _ => _saveManager?.LoadNow();
        }

        public bool TryExecute(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
            {
                return false;
            }

            string[] parts = commandLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string cmd = parts[0].ToLowerInvariant();

            if (!_commands.TryGetValue(cmd, out Action<string[]> action))
            {
                Debug.LogWarning($"Unknown debug command: {cmd}");
                return false;
            }

            action(parts);
            return true;
        }

        private void SetHeat(string[] args)
        {
            if (_heatSystem == null || args.Length < 2 || !int.TryParse(args[1], out int target))
            {
                return;
            }

            for (int i = 0; i < target; i++)
            {
                _heatSystem.CommitCrime(10, 0, 1f);
            }
        }
    }
}
