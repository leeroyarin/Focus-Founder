using UnityEngine;
using System.Collections;
using FocusFounder.Services;
using FocusFounder.Focus;
using FocusFounder.UI;

namespace FocusFounder.Core
{
    /// <summary>
    /// Central game manager that initializes and coordinates all systems
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Service References")]
        [SerializeField] private FocusService focusService;
        [SerializeField] private SimulationClock simulationClock;
        [SerializeField] private EconomyService economyService;
        [SerializeField] private EmployeeService employeeService;
        [SerializeField] private OfficeService officeService;
        [SerializeField] private TaskService taskService;

        [Header("UI References")]
        [SerializeField] private CompanyDashboardVM dashboardViewModel;
        [SerializeField] private CompanyDashboardView dashboardView;

        [Header("Game Settings")]
        [SerializeField] private bool autoSave = true;
        [SerializeField] private float autoSaveIntervalSeconds = 60f;

        private EventBus _eventBus;
        private bool _gameInitialized = false;

        private void Awake()
        {
            // Initialize core systems first
            _eventBus = new EventBus();
            Services.Register<IEventBus>(_eventBus);
        }

        private void Start()
        {
            StartCoroutine(InitializeGameSystems());
        }

        private IEnumerator InitializeGameSystems()
        {
            Debug.Log("Initializing Focus Founder game systems...");

            // 1. Initialize Focus System
            if (focusService != null)
            {
                focusService.Initialize(_eventBus);
                Services.Register<IFocusService>(focusService);
                Debug.Log("✓ Focus Service initialized");
            }

            yield return null;

            // 2. Initialize Simulation Clock
            if (simulationClock != null)
            {
                simulationClock.Initialize(Services.Get<IFocusService>());
                Services.Register<ISimulationClock>(simulationClock);
                Services.Register<ITimeProvider>(simulationClock);
                Debug.Log("✓ Simulation Clock initialized");
            }

            yield return null;

            // 3. Initialize Economy Service
            if (economyService != null)
            {
                Services.Register<IEconomyService>(economyService);
                Debug.Log("✓ Economy Service initialized");
            }

            yield return null;

            // 4. Initialize Employee Service
            if (employeeService != null)
            {
                employeeService.Initialize(
                    Services.Get<IEconomyService>(),
                    Services.Get<ISimulationClock>()
                );
                Services.Register<IEmployeeService>(employeeService);
                Debug.Log("✓ Employee Service initialized");
            }

            yield return null;

            // 5. Initialize Office Service
            if (officeService != null)
            {
                officeService.Initialize(Services.Get<IEconomyService>());
                Services.Register<IOfficeService>(officeService);
                Debug.Log("✓ Office Service initialized");
            }

            yield return null;

            // 6. Initialize Task Service
            if (taskService != null)
            {
                taskService.Initialize(
                    Services.Get<IEconomyService>(),
                    _eventBus
                );
                Services.Register<ITaskService>(taskService);
                Debug.Log("✓ Task Service initialized");
            }

            yield return null;

            // 7. Initialize UI System
            if (dashboardViewModel != null && dashboardView != null)
            {
                dashboardViewModel.Initialize(
                    Services.Get<IEconomyService>(),
                    Services.Get<IEmployeeService>(),
                    Services.Get<IOfficeService>(),
                    Services.Get<IFocusService>(),
                    _eventBus
                );
                dashboardView.Initialize(dashboardViewModel);
                Debug.Log("✓ UI System initialized");
            }

            yield return null;

            // 8. Load saved game data
            yield return StartCoroutine(LoadGameData());

            // 9. Start auto-save coroutine
            if (autoSave)
            {
                StartCoroutine(AutoSaveCoroutine());
            }

            _gameInitialized = true;
            Debug.Log("✓ Game systems initialization complete!");

            // Start the game simulation
            StartGameSimulation();
        }

        private void StartGameSimulation()
        {
            // Start with some default content if this is a new game
            var economyService = Services.Get<IEconomyService>();
            var employeeService = Services.Get<IEmployeeService>();
            var officeService = Services.Get<IOfficeService>();

            // Check if this is a new game (no existing data)
            if (employeeService.GetAllEmployees().Count == 0)
            {
                SetupNewGame();
            }

            Debug.Log("Game simulation started!");
        }

        private void SetupNewGame()
        {
            Debug.Log("Setting up new game...");

            // This would typically load from ScriptableObjects
            // For now, we'll just log that setup would happen here
            Debug.Log("New game setup complete");
        }

        private IEnumerator LoadGameData()
        {
            Debug.Log("Loading game data...");
            // Load save data implementation would go here
            yield return null;
            Debug.Log("Game data loaded");
        }

        private IEnumerator AutoSaveCoroutine()
        {
            while (_gameInitialized)
            {
                yield return new WaitForSeconds(autoSaveIntervalSeconds);
                SaveGame();
            }
        }

        public void SaveGame()
        {
            Debug.Log("Saving game...");
            // Save implementation would go here
            // Would iterate through all ISaveable services and capture their state
        }

        public void LoadGame()
        {
            Debug.Log("Loading game...");
            // Load implementation would go here
        }

        private void Update()
        {
            if (!_gameInitialized) return;

            // Tick all services that need regular updates
            var deltaTime = Services.Get<ITimeProvider>()?.DeltaTime ?? 0f;

            if (deltaTime > 0f) // Only tick when focused
            {
                Services.Get<IEmployeeService>()?.TickAllEmployees(deltaTime);
                Services.Get<IOfficeService>()?.TickAllOffices(deltaTime);
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                SaveGame();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                SaveGame();
        }

        private void OnDestroy()
        {
            if (_gameInitialized)
                SaveGame();
        }

        // Public API for external systems
        public bool IsGameReady => _gameInitialized;

        public void RestartGame()
        {
            Services.Get<ServiceLocator>()?.Clear();
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }
}