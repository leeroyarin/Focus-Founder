# Create the complete Unity script collection
# I'll create all scripts as individual files

# 1. Core Infrastructure Scripts
core_scripts = {}

# IEvent.cs
core_scripts["IEvent.cs"] = '''namespace FocusFounder.Core
{
    /// <summary>
    /// Base interface for all events in the system
    /// </summary>
    public interface IEvent { }
}'''

# IEventBus.cs
core_scripts["IEventBus.cs"] = '''using System;

namespace FocusFounder.Core
{
    /// <summary>
    /// Central event bus for decoupled communication between systems
    /// </summary>
    public interface IEventBus
    {
        void Publish<T>(T evt) where T : IEvent;
        IDisposable Subscribe<T>(Action<T> handler) where T : IEvent;
    }
}'''

# EventBus.cs
core_scripts["EventBus.cs"] = '''using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FocusFounder.Core
{
    /// <summary>
    /// Simple event bus implementation for inter-system communication
    /// </summary>
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscriptions = new();

        public void Publish<T>(T evt) where T : IEvent
        {
            if (_subscriptions.TryGetValue(typeof(T), out var list))
            {
                foreach (var del in list.ToArray())
                {
                    try
                    {
                        ((Action<T>)del)?.Invoke(evt);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error in event handler for {typeof(T).Name}: {ex}");
                    }
                }
            }
        }

        public IDisposable Subscribe<T>(Action<T> handler) where T : IEvent
        {
            if (!_subscriptions.TryGetValue(typeof(T), out var list))
                _subscriptions[typeof(T)] = list = new List<Delegate>();
            
            list.Add(handler);
            return new ActionOnDispose(() => list.Remove(handler));
        }

        private sealed class ActionOnDispose : IDisposable
        {
            private readonly Action _action;
            public ActionOnDispose(Action action) => _action = action;
            public void Dispose() => _action?.Invoke();
        }
    }
}'''

# ITimeProvider.cs
core_scripts["ITimeProvider.cs"] = '''namespace FocusFounder.Core
{
    /// <summary>
    /// Provides time information, scaled by focus state
    /// </summary>
    public interface ITimeProvider
    {
        float DeltaTime { get; }           // scaled by focus
        double NowRealtime { get; }        // unscaled real time
    }
}'''

# ISimulationClock.cs
core_scripts["ISimulationClock.cs"] = '''namespace FocusFounder.Core
{
    /// <summary>
    /// Controls simulation timing and running state
    /// </summary>
    public interface ISimulationClock : ITimeProvider
    {
        bool Running { get; }
        void SetRunning(bool running);
    }
}'''

# SimulationClock.cs
core_scripts["SimulationClock.cs"] = '''using UnityEngine;

namespace FocusFounder.Core
{
    /// <summary>
    /// Central clock that controls all simulation timing
    /// Only advances when the game is focused and running
    /// </summary>
    public sealed class SimulationClock : MonoBehaviour, ISimulationClock
    {
        private bool _running = true;
        private IFocusService _focusService;

        public bool Running => _running;
        public float DeltaTime => _running && _focusService?.IsFocused == true ? Time.deltaTime : 0f;
        public double NowRealtime => Time.realtimeSinceStartupAsDouble;

        public void Initialize(IFocusService focusService)
        {
            _focusService = focusService;
        }

        public void SetRunning(bool running)
        {
            _running = running;
        }
    }
}'''

# ISaveable.cs
core_scripts["ISaveable.cs"] = '''namespace FocusFounder.Core
{
    /// <summary>
    /// Interface for objects that can save/restore their state
    /// </summary>
    public interface ISaveable
    {
        string SaveKey { get; }
        object CaptureState();
        void RestoreState(object state);
    }
}'''

# Print first batch
print("Core Infrastructure Scripts Created:")
for name, content in core_scripts.items():
    print(f"- {name}")
    
# Save to files
import os
os.makedirs("Unity_Scripts/Core", exist_ok=True)
for name, content in core_scripts.items():
    with open(f"Unity_Scripts/Core/{name}", "w") as f:
        f.write(content)