using UnityEngine;

namespace FocusFounder.Animation
{
    /// <summary>
    /// Adapter that implements IAnimPlayable using Unity's Animator system
    /// Maps high-level animation intents to specific Animator triggers
    /// </summary>
    public class AnimatorAdapter : MonoBehaviour, IAnimPlayable
    {
        [SerializeField] private Animator animator;

        [Header("Animation Triggers")]
        [SerializeField] private string workTrigger = "Work";
        [SerializeField] private string idleTrigger = "Idle";
        [SerializeField] private string celebrateTrigger = "Celebrate";
        [SerializeField] private string focusPulseTrigger = "FocusPulse";

        [Header("Animation Parameters")]
        [SerializeField] private string workIntensityFloat = "WorkIntensity";

        private void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        public void PlayWorkLoop()
        {
            if (animator != null)
                animator.SetTrigger(workTrigger);
        }

        public void PlayIdle()
        {
            if (animator != null)
                animator.SetTrigger(idleTrigger);
        }

        public void PlayCelebrate()
        {
            if (animator != null)
                animator.SetTrigger(celebrateTrigger);
        }

        public void PlayFocusPulse()
        {
            if (animator != null)
                animator.SetTrigger(focusPulseTrigger);
        }

        public void SetWorkIntensity(float intensity)
        {
            if (animator != null)
                animator.SetFloat(workIntensityFloat, Mathf.Clamp01(intensity));
        }

        // Direct animator access for custom animations
        public void SetTrigger(string trigger)
        {
            if (animator != null)
                animator.SetTrigger(trigger);
        }

        public void SetFloat(string parameter, float value)
        {
            if (animator != null)
                animator.SetFloat(parameter, value);
        }

        public void SetBool(string parameter, bool value)
        {
            if (animator != null)
                animator.SetBool(parameter, value);
        }
    }
}