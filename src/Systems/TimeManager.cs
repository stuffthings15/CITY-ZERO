using Godot;
using System.Collections.Generic;
using CityZero.Core;
using CityZero.Core.Events;

namespace CityZero.Systems
{
    /// <summary>
    /// Manages in-game time (24-minute real-time day cycle by default).
    /// Broadcasts time events on hour change. Drives district activity density and AI schedules.
    /// </summary>
    public partial class TimeManager : Node
    {
        /// <summary>Seconds per in-game minute (default: 1 real second = 1 in-game minute).</summary>
        [Export] public float RealSecondsPerInGameMinute { get; set; } = 1f;

        /// <summary>Current in-game time in hours (0.0–24.0).</summary>
        public float TimeOfDay { get; private set; } = 8f;   // Start at 8 AM

        private float _accumulatedSeconds = 0f;
        private int _lastHour = -1;

        public override void _Ready()
        {
            ServiceLocator.Register<TimeManager>(this);
        }

        public override void _Process(double delta)
        {
            _accumulatedSeconds += (float)delta;

            float minutesAdvanced = _accumulatedSeconds / RealSecondsPerInGameMinute;
            _accumulatedSeconds -= minutesAdvanced * RealSecondsPerInGameMinute;

            TimeOfDay = (TimeOfDay + minutesAdvanced / 60f) % 24f;

            int currentHour = (int)TimeOfDay;
            if (currentHour != _lastHour)
            {
                _lastHour = currentHour;
                OnHourChanged(currentHour);
            }
        }

        public bool IsNight => TimeOfDay >= 21f || TimeOfDay < 6f;
        public bool IsDay   => TimeOfDay >= 6f && TimeOfDay < 21f;

        public void SetTime(float timeOfDay)
            => TimeOfDay = Godot.Mathf.Clamp(timeOfDay, 0f, 24f);

        private void OnHourChanged(int hour)
        {
            GD.Print($"[TimeManager] Hour changed: {hour:D2}:00");
            // TODO: Emit TimeHourChangedEvent when event added to GameEvents.cs
        }
    }
}
