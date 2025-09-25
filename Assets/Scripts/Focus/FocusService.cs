using System;
using System.Collections;
using UnityEngine;

namespace FocusFounder.Focus
{
    using Core;

    /// <summary>
    /// Manages app focus detection with anti-cheese measures
    /// Only counts focus time when app is truly focused
    /// </summary>
    public sealed class FocusService : MonoBehaviour, IFocusService
    {
        [SerializeField] private float debounceSec = 1.0f;
        [SerializeField] private float minSessionSec = 3.0f;

        private bool _isFocused;
        private DateTimeOffset _focusedAt;
        private TimeSpan _totalFocusTimeToday;
        private DateTime _lastResetDate;
        private IEventBus _eventBus;
        private Coroutine _debounceCoroutine;

        public bool IsFocused => _isFocused;
        public TimeSpan CurrentSessionDuration => _isFocused 
            ? DateTimeOffset.UtcNow - _focusedAt 
            : TimeSpan.Zero;
        public TimeSpan TotalFocusTimeToday => _totalFocusTimeToday + CurrentSessionDuration;

        public void Initialize(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _lastResetDate = DateTime.Now.Date;
        }


        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
                StartGainFocusSequence();
            else
                LoseFocus();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                LoseFocus();
            else
                StartGainFocusSequence();
        }

        private void StartGainFocusSequence()
        {
            if (_debounceCoroutine != null)
                StopCoroutine(_debounceCoroutine);

            _debounceCoroutine = StartCoroutine(GainFocusAfterDebounce());
        }

        private IEnumerator GainFocusAfterDebounce()
        {
            yield return new WaitForSeconds(debounceSec);

            if (Application.isFocused /*&& Application.IsNotRunningInBackground*/)
            {
                GainFocus();
                yield return new WaitForSeconds(minSessionSec);

                if (_isFocused)
                {
                    _eventBus?.Publish(new SessionStarted(_focusedAt));
                }
            }
        }

        private void GainFocus()
        {
            if (_isFocused) return;

            ResetDailyTimeIfNeeded();

            _isFocused = true;
            _focusedAt = DateTimeOffset.UtcNow;
            _eventBus?.Publish(new FocusGained());
        }

        private void LoseFocus()
        {
            if (!_isFocused) return;

            var endTime = DateTimeOffset.UtcNow;
            var sessionDuration = endTime - _focusedAt;

            if (sessionDuration.TotalSeconds >= minSessionSec)
            {
                _totalFocusTimeToday += sessionDuration;
                _eventBus?.Publish(new SessionEnded(endTime, sessionDuration));
            }

            _isFocused = false;
            _eventBus?.Publish(new FocusLost());

            if (_debounceCoroutine != null)
            {
                StopCoroutine(_debounceCoroutine);
                _debounceCoroutine = null;
            }
        }

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