using UnityEngine;
using CityZero.Data.RuntimeDTOs;

namespace CityZero.Data.Importers
{
    public static class ConfigValidator
    {
        public static bool Validate(MissionData mission, out string error)
        {
            if (mission == null)
            {
                error = "Mission is null.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(mission.id))
            {
                error = "Mission id is missing.";
                return false;
            }

            if (mission.objectives == null || mission.objectives.Length == 0)
            {
                error = $"Mission {mission.id} has no objectives.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
