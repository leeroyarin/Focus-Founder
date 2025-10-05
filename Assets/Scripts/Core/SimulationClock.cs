using FocusFounder.Focus;
using UnityEngine;

namespace FocusFounder.Core
{
    /// <summary>
    /// Central clock that controls all simulation timing
    /// Only advances when the game is focused and running
    /// </summary>
    public sealed class SimulationClock : Singleton<SimulationClock>, ISimulationClock
    {
        private bool _running = true;
        private IFocusService _focusService;

        public bool Running => _running;
        public float DeltaTime => _running && _focusService?.IsFocused == true ? Time.deltaTime : 0f;
        public double NowRealtime => Time.realtimeSinceStartupAsDouble;

        public void Initialize(IFocusService focusService)
        {
            _focusService = focusService;
        }

        public void SetRunning(bool running)
        {
            _running = running;
        }
    }
}