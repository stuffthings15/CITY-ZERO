using Godot;
using System;
using System.Collections.Generic;

namespace CityZero.Core
{
    /// <summary>
    /// Global event dispatcher. All systems communicate via GameBus — never hold direct references.
    /// Usage: GameBus.Instance.Emit(new HeatChangedEvent(...))
    ///        GameBus.Instance.Subscribe<HeatChangedEvent>(OnHeatChanged)
    /// </summary>
    public partial class GameBus : Node
    {
        public static GameBus Instance { get; private set; }

        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public override void _Ready()
        {
            if (Instance != null)
            {
                QueueFree();
                return;
            }
            Instance = this;
        }

        public void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
                _subscribers[type] = new List<Delegate>();
            _subscribers[type].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var list))
                list.Remove(handler);
        }

        public void Emit<T>(T gameEvent) where T : IGameEvent
        {
            var type = typeof(T);
            if (!_subscribers.TryGetValue(type, out var list)) return;

            // Iterate a snapshot to allow unsub during dispatch
            foreach (var del in list.ToArray())
                (del as Action<T>)?.Invoke(gameEvent);
        }
    }

    public interface IGameEvent { }
}
