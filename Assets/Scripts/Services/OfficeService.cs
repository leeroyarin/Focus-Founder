using System.Collections.Generic;
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
    public class OfficeService : Singleton<OfficeService>, IOfficeService, ISaveable
    {
        private List<Office> _allOffices = new();
        private Dictionary<string, Office> _officeById = new();
        private Dictionary<string, string> _employeeToOffice = new(); // employee id -> office id

        private IEconomyService _economyService;

        public event System.Action<Office> OnOfficeUnlocked;
        public event System.Action<Office, Employee> OnEmployeeMovedToOffice;

        [field: SerializeField]
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

        public void InitializeOnSaver() => Services.Get<SaveService>().RegisterSavableObjects(this);

        [System.Serializable]
        private struct OfficeServiceSaveData
        {
            public object[] offices;
            public System.Collections.Generic.KeyValuePair<string, string>[] employeeToOffice;
        }
    }
}