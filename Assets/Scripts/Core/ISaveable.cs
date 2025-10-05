namespace FocusFounder.Core
{
    /// <summary>
    /// Interface for objects that can save/restore their state
    /// </summary>
    public interface ISaveable
    {
        string SaveKey { get; }
        void InitializeOnSaver();
        object CaptureState();
        void RestoreState(object state);
    }
}