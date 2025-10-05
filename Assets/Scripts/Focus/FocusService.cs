using System;
using System.Collections;
using UnityEngine;
using FocusFounder.Core;
namespace FocusFounder.Focus
{

    /// <summary>
    /// Manages app focus detection with anti-cheese measures.
    /// Only counts focus time when app is truly focused.
    /// </summary>
    public sealed class FocusService : Singleton<FocusService>, IFocusService
    {
        // Time to wait before confirming focus gain (debounce period)
        [SerializeField] private float debounceSec = 1.0f;
        // Minimum session duration to be counted as a valid focus session
        [SerializeField] private float minSessionSec = 3.0f;

        // Tracks whether the app is currently focused
        private bool _isFocused;
        // Timestamp when focus was gained
        private DateTimeOffset _focusedAt;
        // Accumulated focus time for today
        private TimeSpan _totalFocusTimeToday;
        // Date when daily focus time was last reset
        private DateTime _lastResetDate;
        // Reference to the event bus for publishing focus/session events
        private IEventBus _eventBus;
        // Reference to the running debounce coroutine
        private Coroutine _debounceCoroutine;

        // Indicates if the app is currently focused
        public bool IsFocused => _isFocused;
        // Duration of the current focus session (if focused)
        public TimeSpan CurrentSessionDuration => _isFocused 
            ? DateTimeOffset.UtcNow - _focusedAt 
            : TimeSpan.Zero;
        // Total focus time for today, including the current session if focused
        public TimeSpan TotalFocusTimeToday => _totalFocusTimeToday + CurrentSessionDuration;

        /// <summary>
        /// Initializes the service with the event bus and resets daily time.
        /// </summary>
        public void Initialize(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _lastResetDate = DateTime.Now.Date;
        }

        /// <summary>
        /// Called by Unity when app focus changes.
        /// Starts or ends focus session accordingly.
        /// </summary>
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
                StartGainFocusSequence(); // Begin debounce before gaining focus
            else
                LoseFocus(); // End focus session immediately
        }

        /// <summary>
        /// Called by Unity when app is paused or resumed.
        /// Handles focus logic similar to OnApplicationFocus.
        /// </summary>
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                LoseFocus();
            else
                StartGainFocusSequence();
        }

        /// <summary>
        /// Starts the debounce coroutine to confirm focus after a delay.
        /// </summary>
        private void StartGainFocusSequence()
        {
            if (_debounceCoroutine != null)
                StopCoroutine(_debounceCoroutine); // Stop any running debounce

            _debounceCoroutine = StartCoroutine(GainFocusAfterDebounce());
        }

        /// <summary>
        /// Coroutine that waits for debounce period, then confirms focus.
        /// If still focused, starts a session after minSessionSec.
        /// </summary>
        private IEnumerator GainFocusAfterDebounce()
        {
            yield return new WaitForSeconds(debounceSec);

            // Check if app is still focused after debounce
            if (Application.isFocused /*&& Application.IsNotRunningInBackground*/)
            {
                GainFocus(); // Mark as focused
                yield return new WaitForSeconds(minSessionSec);

                // If still focused after minimum session time, publish session started event
                if (_isFocused)
                {
                    _eventBus?.Publish(new SessionStarted(_focusedAt));
                }
            }
        }

        /// <summary>
        /// Marks the app as focused, resets daily time if needed, and publishes event.
        /// </summary>
        private void GainFocus()
        {
            if (_isFocused) return; // Already focused

            ResetDailyTimeIfNeeded();

            _isFocused = true;
            _focusedAt = DateTimeOffset.UtcNow;
            _eventBus?.Publish(new FocusGained());
        }

        /// <summary>
        /// Ends the current focus session, updates total time, and publishes events.
        /// </summary>
        private void LoseFocus()
        {
            if (!_isFocused) return; // Not focused, nothing to do

            var endTime = DateTimeOffset.UtcNow;
            var sessionDuration = endTime - _focusedAt;

            // Only count session if it meets minimum duration
            if (sessionDuration.TotalSeconds >= minSessionSec)
            {
                _totalFocusTimeToday += sessionDuration;
                _eventBus?.Publish(new SessionEnded(endTime, sessionDuration));
            }

            _isFocused = false;
            _eventBus?.Publish(new FocusLost());

            // Stop debounce coroutine if running
            if (_debounceCoroutine != null)
            {
                StopCoroutine(_debounceCoroutine);
                _debounceCoroutine = null;
            }
        }

        /// <summary>
        /// Resets the daily focus time if the date has changed.
        /// </summary>
        private void ResetDailyTimeIfNeeded()
        {
            var today = DateTime.Now.Date;
            if (today != _lastResetDate)
            {
                _totalFocusTimeToday = TimeSpan.Zero;
                _lastResetDate = today;
            }
        }
    }
}