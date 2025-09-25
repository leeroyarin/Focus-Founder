using System;

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
}