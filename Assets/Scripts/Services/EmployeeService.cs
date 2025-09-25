using System.Collections.Generic;
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
}