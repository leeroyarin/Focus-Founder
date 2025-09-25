# 7. Animation System
animation_scripts = {}

# IAnimPlayable.cs
animation_scripts["IAnimPlayable.cs"] = '''namespace FocusFounder.Animation
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
}'''

# IAnimationBridge.cs
animation_scripts["IAnimationBridge.cs"] = '''namespace FocusFounder.Animation
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
}'''

# AnimatorAdapter.cs
animation_scripts["AnimatorAdapter.cs"] = '''using UnityEngine;

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
}'''

# EmployeeView.cs
animation_scripts["EmployeeView.cs"] = '''using UnityEngine;

namespace FocusFounder.Animation
{
    using Domain;
    using Core;

    /// <summary>
    /// Visual representation of an employee with animation integration
    /// Responds to employee state changes and triggers appropriate animations
    /// </summary>
    public class EmployeeView : MonoBehaviour
    {
        [SerializeField] private AnimatorAdapter animatorAdapter;
        [SerializeField] private SpriteRenderer characterSprite;
        [SerializeField] private Transform workEffectAnchor;
        
        private Employee _employee;
        private EmployeeState _lastState = EmployeeState.Idle;
        private IEventBus _eventBus;

        public Employee Employee => _employee;
        public IAnimPlayable AnimatorAdapter => animatorAdapter;

        public void Initialize(Employee employee, IEventBus eventBus)
        {
            _employee = employee;
            _eventBus = eventBus;
            
            // Set visual properties from employee archetype
            if (employee.Archetype.portrait != null)
                characterSprite.sprite = employee.Archetype.portrait;
            
            characterSprite.color = employee.Archetype.characterColor;
        }

        private void Update()
        {
            if (_employee == null) return;

            // Check for state changes
            if (_employee.State != _lastState)
            {
                OnStateChanged(_employee.State);
                _lastState = _employee.State;
            }

            // Update work intensity based on productivity
            if (_employee.State == EmployeeState.Working)
            {
                var intensity = Mathf.Clamp01(_employee.Stats.productivity / 2f);
                animatorAdapter?.SetWorkIntensity(intensity);
            }
        }

        private void OnStateChanged(EmployeeState newState)
        {
            switch (newState)
            {
                case EmployeeState.Idle:
                    animatorAdapter?.PlayIdle();
                    break;
                
                case EmployeeState.Working:
                    animatorAdapter?.PlayWorkLoop();
                    break;
                
                case EmployeeState.Celebrating:
                    animatorAdapter?.PlayCelebrate();
                    // Auto-return to idle after celebration
                    Invoke(nameof(ReturnToIdle), 2f);
                    break;
                
                case EmployeeState.Break:
                    animatorAdapter?.PlayIdle();
                    break;
            }
        }

        private void ReturnToIdle()
        {
            if (_employee != null && _employee.State == EmployeeState.Celebrating)
            {
                _employee.SetIdle();
            }
        }

        public void OnTaskCompleted()
        {
            // Visual feedback for task completion
            animatorAdapter?.PlayCelebrate();
            
            // Could trigger particle effects, sound, etc.
            ShowCompletionEffect();
        }

        private void ShowCompletionEffect()
        {
            // Placeholder for completion VFX
            // Could instantiate particle systems, floating text, etc.
        }

        // Focus-related animations
        public void OnFocusGained()
        {
            animatorAdapter?.PlayFocusPulse();
        }

        // Morale-based visual feedback
        public void UpdateMoraleVisuals()
        {
            if (_employee == null) return;

            // Adjust sprite color based on morale
            var moralePercent = _employee.Morale / 100f;
            var moraleColor = Color.Lerp(Color.gray, Color.white, moralePercent);
            characterSprite.color = Color.Lerp(characterSprite.color, moraleColor, Time.deltaTime);
        }
    }
}'''

# Save Animation scripts
import os
os.makedirs("Unity_Scripts/Animation", exist_ok=True)
for name, content in animation_scripts.items():
    with open(f"Unity_Scripts/Animation/{name}", "w") as f:
        f.write(content)

print("Animation System Scripts Created:")
for name in animation_scripts.keys():
    print(f"- {name}")