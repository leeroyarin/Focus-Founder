using System;
using UnityEngine;

namespace FocusFounder.UI
{
    using Core;
    using Domain;
    using Services;
    using Focus;

    /// <summary>
    /// ViewModel for the main company dashboard
    /// </summary>
    public class CompanyDashboardVM : MonoBehaviour, IViewModel
    {
        [Header("Bindable Properties")]
        public ObservableProperty<float> Cash = new();
        public ObservableProperty<float> Research = new();
        public ObservableProperty<float> Reputation = new();
        public ObservableProperty<TimeSpan> FocusTime = new();
        public ObservableProperty<bool> IsFocused = new();
        public ObservableProperty<int> EmployeeCount = new();
        public ObservableProperty<int> OfficeCount = new();
        public ObservableCollection<Employee> Employees = new();

        private IEconomyService _economyService;
        private IEmployeeService _employeeService;
        private IOfficeService _officeService;
        private IFocusService _focusService;
        private IEventBus _eventBus;

        private IDisposable _focusGainedSub;
        private IDisposable _focusLostSub;
        private IDisposable _balanceChangedSub;

        public void Initialize(
            IEconomyService economyService,
            IEmployeeService employeeService, 
            IOfficeService officeService,
            IFocusService focusService,
            IEventBus eventBus)
        {
            _economyService = economyService;
            _employeeService = employeeService;
            _officeService = officeService;
            _focusService = focusService;
            _eventBus = eventBus;
        }

        public void Bind()
        {
            // Subscribe to service events
            _economyService.OnBalanceChanged += OnBalanceChanged;
            _employeeService.OnEmployeeHired += OnEmployeeHired;

            _focusGainedSub = _eventBus.Subscribe<FocusGained>(OnFocusGained);
            _focusLostSub = _eventBus.Subscribe<FocusLost>(OnFocusLost);

            // Initialize values
            RefreshAllData();
        }

        public void Unbind()
        {
            // Unsubscribe from events
            if (_economyService != null)
                _economyService.OnBalanceChanged -= OnBalanceChanged;

            if (_employeeService != null)
                _employeeService.OnEmployeeHired -= OnEmployeeHired;

            _focusGainedSub?.Dispose();
            _focusLostSub?.Dispose();
        }

        private void Update()
        {
            if (_focusService != null)
            {
                FocusTime.Value = _focusService.CurrentSessionDuration;
                IsFocused.Value = _focusService.IsFocused;
            }
        }

        private void RefreshAllData()
        {
            if (_economyService != null)
            {
                var balance = _economyService.GetBalances();
                Cash.Value = balance.cash;
                Research.Value = balance.research;
                Reputation.Value = balance.reputation;
            }

            if (_employeeService != null)
            {
                var employees = _employeeService.GetAllEmployees();
                EmployeeCount.Value = employees.Count;

                Employees.Clear();
                foreach (var employee in employees)
                    Employees.Add(employee);
            }

            if (_officeService != null)
            {
                OfficeCount.Value = _officeService.GetAllOffices().Count;
            }
        }

        private void OnBalanceChanged(CurrencyBalance balance)
        {
            Cash.Value = balance.cash;
            Research.Value = balance.research;
            Reputation.Value = balance.reputation;
        }

        private void OnEmployeeHired(Employee employee)
        {
            Employees.Add(employee);
            EmployeeCount.Value = Employees.Count;
        }

        private void OnFocusGained(FocusGained evt)
        {
            IsFocused.Value = true;
        }

        private void OnFocusLost(FocusLost evt)
        {
            IsFocused.Value = false;
        }
    }
}