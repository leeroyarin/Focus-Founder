using System;
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
}