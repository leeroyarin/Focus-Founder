# Continue with more services

# IEmployeeService.cs
service_scripts["IEmployeeService.cs"] = '''using System.Collections.Generic;

namespace FocusFounder.Services
{
    using Domain;
    using Data;

    /// <summary>
    /// Manages employee hiring, assignment, and progression
    /// </summary>
    public interface IEmployeeService
    {
        List<Employee> GetAllEmployees();
        Employee GetEmployee(string id);
        bool TryHireEmployee(EmployeeArchetypeSO archetype, Office office);
        bool TryAssignToOffice(Employee employee, Office office);
        void TickAllEmployees(float deltaTime);
        
        // Events
        event System.Action<Employee> OnEmployeeHired;
        event System.Action<Employee, Office> OnEmployeeAssigned;
        event System.Action<Employee> OnEmployeeLevelUp;
    }
}'''

# EmployeeService.cs
service_scripts["EmployeeService.cs"] = '''using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FocusFounder.Services
{
    using Core;
    using Domain;
    using Data;

    /// <summary>
    /// Handles all employee management operations
    /// </summary>
    public class EmployeeService : MonoBehaviour, IEmployeeService, ISaveable
    {
        private List<Employee> _allEmployees = new();
        private Dictionary<string, Employee> _employeeById = new();
        
        private IEconomyService _economyService;
        private ISimulationClock _clock;
        
        public event System.Action<Employee> OnEmployeeHired;
        public event System.Action<Employee, Office> OnEmployeeAssigned;
        public event System.Action<Employee> OnEmployeeLevelUp;

        public string SaveKey => "Employees";

        public void Initialize(IEconomyService economyService, ISimulationClock clock)
        {
            _economyService = economyService;
            _clock = clock;
        }

        public List<Employee> GetAllEmployees() => _allEmployees.ToList();

        public Employee GetEmployee(string id)
        {
            _employeeById.TryGetValue(id, out var employee);
            return employee;
        }

        public bool TryHireEmployee(EmployeeArchetypeSO archetype, Office office)
        {
            // Calculate hiring cost (could be dynamic based on level, market conditions, etc.)
            var hiringCost = CalculateHiringCost(archetype);
            
            if (!_economyService.TrySpend(hiringCost))
                return false;

            var employee = new Employee(archetype);
            _allEmployees.Add(employee);
            _employeeById[employee.Id] = employee;
            
            if (office != null && office.TryAddEmployee(employee))
            {
                OnEmployeeAssigned?.Invoke(employee, office);
            }
            
            OnEmployeeHired?.Invoke(employee);
            return true;
        }

        public bool TryAssignToOffice(Employee employee, Office office)
        {
            if (office.TryAddEmployee(employee))
            {
                OnEmployeeAssigned?.Invoke(employee, office);
                return true;
            }
            return false;
        }

        public void TickAllEmployees(float deltaTime)
        {
            foreach (var employee in _allEmployees)
            {
                var previousLevel = employee.Level;
                employee.Tick(deltaTime, _clock, _economyService);
                
                if (employee.Level > previousLevel)
                {
                    OnEmployeeLevelUp?.Invoke(employee);
                }
            }
        }

        private CostBundle CalculateHiringCost(EmployeeArchetypeSO archetype)
        {
            // Base cost modified by archetype stats and current employee count
            var baseCost = 100f;
            var countMultiplier = 1f + (_allEmployees.Count * 0.1f);
            var archetypeMultiplier = (archetype.baseStats.productivity + archetype.baseStats.efficiency) / 2f;
            
            var finalCost = baseCost * countMultiplier * archetypeMultiplier;
            
            return new CostBundle(finalCost, 0f, 0f);
        }

        public object CaptureState()
        {
            return new EmployeeServiceSaveData
            {
                employees = _allEmployees.Select(e => e.CaptureState()).ToArray()
            };
        }

        public void RestoreState(object state)
        {
            if (state is EmployeeServiceSaveData data)
            {
                _allEmployees.Clear();
                _employeeById.Clear();
                
                foreach (var employeeState in data.employees)
                {
                    // Would need to reconstruct employees from save data
                    // This requires access to ContentService to resolve archetypes
                }
            }
        }

        [System.Serializable]
        private struct EmployeeServiceSaveData
        {
            public object[] employees;
        }
    }
}'''

# IOfficeService.cs
service_scripts["IOfficeService.cs"] = '''using System.Collections.Generic;

namespace FocusFounder.Services
{
    using Domain;
    using Data;

    /// <summary>
    /// Manages office operations, layouts, and unlocking
    /// </summary>
    public interface IOfficeService
    {
        List<Office> GetAllOffices();
        Office GetOffice(string id);
        bool TryUnlockOffice(OfficeDefinitionSO definition);
        Office GetOfficeForEmployee(Employee employee);
        void TickAllOffices(float deltaTime);
        
        // Events
        event System.Action<Office> OnOfficeUnlocked;
        event System.Action<Office, Employee> OnEmployeeMovedToOffice;
    }
}'''

# OfficeService.cs
service_scripts["OfficeService.cs"] = '''using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FocusFounder.Services
{
    using Core;
    using Domain;
    using Data;

    /// <summary>
    /// Manages all office-related operations
    /// </summary>
    public class OfficeService : MonoBehaviour, IOfficeService, ISaveable
    {
        private List<Office> _allOffices = new();
        private Dictionary<string, Office> _officeById = new();
        private Dictionary<string, string> _employeeToOffice = new(); // employee id -> office id
        
        private IEconomyService _economyService;
        
        public event System.Action<Office> OnOfficeUnlocked;
        public event System.Action<Office, Employee> OnEmployeeMovedToOffice;

        public string SaveKey => "Offices";

        public void Initialize(IEconomyService economyService)
        {
            _economyService = economyService;
        }

        public List<Office> GetAllOffices() => _allOffices.ToList();

        public Office GetOffice(string id)
        {
            _officeById.TryGetValue(id, out var office);
            return office;
        }

        public bool TryUnlockOffice(OfficeDefinitionSO definition)
        {
            if (!_economyService.TrySpend(definition.unlockCost))
                return false;

            var office = new Office(definition);
            _allOffices.Add(office);
            _officeById[office.Id] = office;
            
            OnOfficeUnlocked?.Invoke(office);
            return true;
        }

        public Office GetOfficeForEmployee(Employee employee)
        {
            if (_employeeToOffice.TryGetValue(employee.Id, out var officeId))
            {
                return GetOffice(officeId);
            }
            return null;
        }

        public void TickAllOffices(float deltaTime)
        {
            // Update office-level systems, decorations, etc.
            foreach (var office in _allOffices)
            {
                // Could handle office-wide effects, environmental bonuses, etc.
            }
        }

        public void AssignEmployeeToOffice(Employee employee, Office office)
        {
            // Remove from previous office
            if (_employeeToOffice.TryGetValue(employee.Id, out var oldOfficeId))
            {
                var oldOffice = GetOffice(oldOfficeId);
                oldOffice?.TryRemoveEmployee(employee);
            }
            
            // Add to new office
            if (office.TryAddEmployee(employee))
            {
                _employeeToOffice[employee.Id] = office.Id;
                OnEmployeeMovedToOffice?.Invoke(office, employee);
            }
        }

        public object CaptureState()
        {
            return new OfficeServiceSaveData
            {
                offices = _allOffices.Select(o => o.CaptureState()).ToArray(),
                employeeToOffice = _employeeToOffice.ToArray()
            };
        }

        public void RestoreState(object state)
        {
            if (state is OfficeServiceSaveData data)
            {
                _allOffices.Clear();
                _officeById.Clear();
                _employeeToOffice = data.employeeToOffice.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
                // Would need to reconstruct offices from save data
            }
        }

        [System.Serializable]
        private struct OfficeServiceSaveData
        {
            public object[] offices;
            public System.Collections.Generic.KeyValuePair<string, string>[] employeeToOffice;
        }
    }
}'''

# Save additional service scripts
for name, content in service_scripts.items():
    with open(f"Unity_Scripts/Services/{name}", "w") as f:
        f.write(content)

print("Additional Service Scripts Created:")
for name in ["IEmployeeService.cs", "EmployeeService.cs", "IOfficeService.cs", "OfficeService.cs"]:
    print(f"- {name}")