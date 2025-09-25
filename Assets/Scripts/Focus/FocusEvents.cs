using System;

namespace FocusFounder.Focus
{
    using Core;

    /// <summary>
    /// Event fired when app gains focus
    /// </summary>
    public readonly struct FocusGained : IEvent { }

    /// <summary>
    /// Event fired when app loses focus
    /// </summary>
    public readonly struct FocusLost : IEvent { }

    /// <summary>
    /// Event fired when a focus session starts (after debounce)
    /// </summary>
    public readonly struct SessionStarted : IEvent 
    { 
        public DateTimeOffset At { get; }
        public SessionStarted(DateTimeOffset at) => At = at;
    }

    /// <summary>
    /// Event fired when a focus session ends
    /// </summary>
    public readonly struct SessionEnded : IEvent 
    { 
        public DateTimeOffset At { get; }
        public TimeSpan Duration { get; }

        public SessionEnded(DateTimeOffset at, TimeSpan duration)
        {
            At = at;
            Duration = duration;
        }
    }
}