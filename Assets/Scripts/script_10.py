# 8. UI System (MVVM-like pattern)
ui_scripts = {}

# ObservableProperty.cs
ui_scripts["ObservableProperty.cs"] = '''using System;

namespace FocusFounder.UI
{
    /// <summary>
    /// Observable property for data binding in UI
    /// </summary>
    [Serializable]
    public class ObservableProperty<T>
    {
        private T _value;
        
        public event Action<T> OnValueChanged;

        public T Value
        {
            get => _value;
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    OnValueChanged?.Invoke(_value);
                }
            }
        }

        public ObservableProperty(T initialValue = default)
        {
            _value = initialValue;
        }

        public static implicit operator T(ObservableProperty<T> property)
        {
            return property.Value;
        }
    }
}'''

# ObservableCollection.cs
ui_scripts["ObservableCollection.cs"] = '''using System;
using System.Collections.Generic;
using System.Collections;

namespace FocusFounder.UI
{
    /// <summary>
    /// Observable collection for UI list binding
    /// </summary>
    public class ObservableCollection<T> : IEnumerable<T>
    {
        private readonly List<T> _items = new();
        
        public event Action<T> OnItemAdded;
        public event Action<T> OnItemRemoved;
        public event Action OnCollectionChanged;

        public int Count => _items.Count;
        public T this[int index] => _items[index];

        public void Add(T item)
        {
            _items.Add(item);
            OnItemAdded?.Invoke(item);
            OnCollectionChanged?.Invoke();
        }

        public bool Remove(T item)
        {
            if (_items.Remove(item))
            {
                OnItemRemoved?.Invoke(item);
                OnCollectionChanged?.Invoke();
                return true;
            }
            return false;
        }

        public void Clear()
        {
            var itemsToRemove = new List<T>(_items);
            _items.Clear();
            
            foreach (var item in itemsToRemove)
                OnItemRemoved?.Invoke(item);
            
            OnCollectionChanged?.Invoke();
        }

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}'''

# IViewModel.cs
ui_scripts["IViewModel.cs"] = '''namespace FocusFounder.UI
{
    /// <summary>
    /// Base interface for all ViewModels in the MVVM pattern
    /// </summary>
    public interface IViewModel
    {
        void Bind();
        void Unbind();
    }
}'''

# CompanyDashboardVM.cs
ui_scripts["CompanyDashboardVM.cs"] = '''using System;
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
}'''

# CompanyDashboardView.cs  
ui_scripts["CompanyDashboardView.cs"] = '''using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FocusFounder.UI
{
    /// <summary>
    /// View component for the main company dashboard
    /// Binds to CompanyDashboardVM and updates UI elements
    /// </summary>
    public class CompanyDashboardView : MonoBehaviour
    {
        [Header("Currency Display")]
        [SerializeField] private TextMeshProUGUI cashText;
        [SerializeField] private TextMeshProUGUI researchText;
        [SerializeField] private TextMeshProUGUI reputationText;

        [Header("Focus Display")]
        [SerializeField] private TextMeshProUGUI focusTimeText;
        [SerializeField] private Image focusIndicator;
        [SerializeField] private Color focusedColor = Color.green;
        [SerializeField] private Color unfocusedColor = Color.red;

        [Header("Company Stats")]
        [SerializeField] private TextMeshProUGUI employeeCountText;
        [SerializeField] private TextMeshProUGUI officeCountText;

        [Header("Employee List")]
        [SerializeField] private Transform employeeListParent;
        [SerializeField] private GameObject employeeItemPrefab;

        private CompanyDashboardVM _viewModel;

        public void Initialize(CompanyDashboardVM viewModel)
        {
            _viewModel = viewModel;
        }

        private void OnEnable()
        {
            if (_viewModel != null)
            {
                _viewModel.Bind();
                BindToViewModel();
            }
        }

        private void OnDisable()
        {
            if (_viewModel != null)
            {
                UnbindFromViewModel();
                _viewModel.Unbind();
            }
        }

        private void BindToViewModel()
        {
            // Bind currency values
            _viewModel.Cash.OnValueChanged += OnCashChanged;
            _viewModel.Research.OnValueChanged += OnResearchChanged;
            _viewModel.Reputation.OnValueChanged += OnReputationChanged;

            // Bind focus state
            _viewModel.FocusTime.OnValueChanged += OnFocusTimeChanged;
            _viewModel.IsFocused.OnValueChanged += OnFocusStateChanged;

            // Bind company stats
            _viewModel.EmployeeCount.OnValueChanged += OnEmployeeCountChanged;
            _viewModel.OfficeCount.OnValueChanged += OnOfficeCountChanged;

            // Bind employee collection
            _viewModel.Employees.OnItemAdded += OnEmployeeAdded;
            _viewModel.Employees.OnItemRemoved += OnEmployeeRemoved;
            _viewModel.Employees.OnCollectionChanged += OnEmployeeCollectionChanged;

            // Initialize display with current values
            OnCashChanged(_viewModel.Cash.Value);
            OnResearchChanged(_viewModel.Research.Value);
            OnReputationChanged(_viewModel.Reputation.Value);
            OnFocusStateChanged(_viewModel.IsFocused.Value);
            OnEmployeeCountChanged(_viewModel.EmployeeCount.Value);
            OnOfficeCountChanged(_viewModel.OfficeCount.Value);
        }

        private void UnbindFromViewModel()
        {
            if (_viewModel == null) return;

            _viewModel.Cash.OnValueChanged -= OnCashChanged;
            _viewModel.Research.OnValueChanged -= OnResearchChanged;
            _viewModel.Reputation.OnValueChanged -= OnReputationChanged;
            _viewModel.FocusTime.OnValueChanged -= OnFocusTimeChanged;
            _viewModel.IsFocused.OnValueChanged -= OnFocusStateChanged;
            _viewModel.EmployeeCount.OnValueChanged -= OnEmployeeCountChanged;
            _viewModel.OfficeCount.OnValueChanged -= OnOfficeCountChanged;
            _viewModel.Employees.OnItemAdded -= OnEmployeeAdded;
            _viewModel.Employees.OnItemRemoved -= OnEmployeeRemoved;
            _viewModel.Employees.OnCollectionChanged -= OnEmployeeCollectionChanged;
        }

        private void OnCashChanged(float value)
        {
            if (cashText != null)
                cashText.text = $"${value:F0}";
        }

        private void OnResearchChanged(float value)
        {
            if (researchText != null)
                researchText.text = $"{value:F0} RP";
        }

        private void OnReputationChanged(float value)
        {
            if (reputationText != null)
                reputationText.text = $"{value:F0} REP";
        }

        private void OnFocusTimeChanged(System.TimeSpan time)
        {
            if (focusTimeText != null)
                focusTimeText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
        }

        private void OnFocusStateChanged(bool isFocused)
        {
            if (focusIndicator != null)
                focusIndicator.color = isFocused ? focusedColor : unfocusedColor;
        }

        private void OnEmployeeCountChanged(int count)
        {
            if (employeeCountText != null)
                employeeCountText.text = $"Employees: {count}";
        }

        private void OnOfficeCountChanged(int count)
        {
            if (officeCountText != null)
                officeCountText.text = $"Offices: {count}";
        }

        private void OnEmployeeAdded(Domain.Employee employee)
        {
            CreateEmployeeListItem(employee);
        }

        private void OnEmployeeRemoved(Domain.Employee employee)
        {
            // Find and destroy the corresponding UI element
            // Implementation would track employee UI items
        }

        private void OnEmployeeCollectionChanged()
        {
            // Rebuild entire employee list if needed
            RebuildEmployeeList();
        }

        private void CreateEmployeeListItem(Domain.Employee employee)
        {
            if (employeeItemPrefab != null && employeeListParent != null)
            {
                var item = Instantiate(employeeItemPrefab, employeeListParent);
                // Configure employee item UI with employee data
                var itemText = item.GetComponentInChildren<TextMeshProUGUI>();
                if (itemText != null)
                    itemText.text = employee.Archetype.displayName;
            }
        }

        private void RebuildEmployeeList()
        {
            // Clear existing items
            for (int i = employeeListParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(employeeListParent.GetChild(i).gameObject);
            }

            // Recreate items
            foreach (var employee in _viewModel.Employees)
            {
                CreateEmployeeListItem(employee);
            }
        }
    }
}'''

# Save UI scripts
import os
os.makedirs("Unity_Scripts/UI", exist_ok=True)
for name, content in ui_scripts.items():
    with open(f"Unity_Scripts/UI/{name}", "w") as f:
        f.write(content)

print("UI System Scripts Created:")
for name in ui_scripts.keys():
    print(f"- {name}")