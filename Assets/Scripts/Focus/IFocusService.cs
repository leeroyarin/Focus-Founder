using System;

namespace FocusFounder.Focus
{
    /// <summary>
    /// Service that tracks app focus state and manages sessions
    /// </summary>
    public interface IFocusService
    {
        bool IsFocused { get; }
        TimeSpan CurrentSessionDuration { get; }
        TimeSpan TotalFocusTimeToday { get; }
    }

    public interface ISingletonable { }
}