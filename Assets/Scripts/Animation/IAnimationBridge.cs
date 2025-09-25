namespace FocusFounder.Animation
{
    /// <summary>
    /// Bridge between game logic and Unity's animation systems
    /// Allows swapping animation backends without code changes
    /// </summary>
    public interface IAnimationBridge
    {
        void Bind(IAnimPlayable animatable);
        void Trigger(string triggerName);
        void SetFloat(string parameterName, float value);
        void SetBool(string parameterName, bool value);
        void SetInt(string parameterName, int value);
    }
}