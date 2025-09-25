using UnityEngine;

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
}