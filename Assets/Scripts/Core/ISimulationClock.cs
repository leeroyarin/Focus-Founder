namespace FocusFounder.Core
{
    /// <summary>
    /// Controls simulation timing and running state
    /// </summary>
    public interface ISimulationClock : ITimeProvider
    {
        bool Running { get; }
        void SetRunning(bool running);
    }
}