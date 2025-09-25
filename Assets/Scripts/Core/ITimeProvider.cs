namespace FocusFounder.Core
{
    /// <summary>
    /// Provides time information, scaled by focus state
    /// </summary>
    public interface ITimeProvider
    {
        float DeltaTime { get; }           // scaled by focus
        double NowRealtime { get; }        // unscaled real time
    }
}