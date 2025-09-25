namespace FocusFounder.Animation
{
    /// <summary>
    /// High-level animation interface that abstracts Unity's animation systems
    /// Provides intent-based animation triggers rather than direct clip names
    /// </summary>
    public interface IAnimPlayable
    {
        void PlayWorkLoop();        // Employee working animation
        void PlayIdle();            // Employee idle animation  
        void PlayCelebrate();       // Employee celebration animation
        void PlayFocusPulse();      // UI glow when focus starts
        void SetWorkIntensity(float intensity); // 0-1 for work animation speed
    }
}