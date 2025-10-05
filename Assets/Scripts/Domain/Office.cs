using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FocusFounder.Domain
{
    using Core;
    using Data;
    using FocusFounder.Services;

    [Serializable]
    public sealed class Office : ISaveable
    {
        [SerializeField] private string _id;
        [SerializeField] private List<Employee> _staff = new();

        public string Id => _id;
        public OfficeDefinitionSO Definition { get; private set; }
        public List<Employee> Staff => _staff;
        public OfficeLayout Layout { get; private set; }
        public OfficeModifiers Modifiers { get; private set; }

        public int MaxStaff => Definition.maxStaff;
        public int CurrentStaff => _staff.Count;
        public bool IsFull => CurrentStaff >= MaxStaff;

        public string SaveKey => $"Office_{_id}";

        public Office(OfficeDefinitionSO definition)
        {
            _id = Guid.NewGuid().ToString();
            Definition = definition;
            Layout = new OfficeLayout(definition.gridSize);
            Modifiers = new OfficeModifiers();
        }

        public bool TryAddEmployee(Employee employee)
        {
            if (!IsFull && !_staff.Contains(employee))
            {
                _staff.Add(employee);
                return true;
            }
            return false;
        }

        public bool TryRemoveEmployee(Employee employee)
        {
            return _staff.Remove(employee);
        }

        public List<Employee> GetIdleEmployees()
        {
            return _staff.Where(e => e.State == EmployeeState.Idle).ToList();
        }

        public List<Employee> GetWorkingEmployees()
        {
            return _staff.Where(e => e.State == EmployeeState.Working).ToList();
        }

        public EmployeeStats GetCombinedStats()
        {
            var combined = new EmployeeStats();
            foreach (var employee in _staff)
            {
                combined += employee.Stats;
            }
            return combined * Modifiers.StatsMultiplier;
        }

        public object CaptureState()
        {
            return new OfficeSaveData
            {
                id = _id,
                definitionId = Definition.id,
                staffIds = _staff.Select(s => s.Id).ToArray(),
                // Layout and Modifiers would need their own serialization
            };
        }

        public void RestoreState(object state)
        {
            if (state is OfficeSaveData data)
            {
                _id = data.id;
                // Definition and staff should be resolved by services
            }
        }

        public void InitializeOnSaver()
        {
            Singleton<SaveService>.Instance.RegisterSavableObjects(this);
        }

        [Serializable]
        private struct OfficeSaveData
        {
            public string id;
            public string definitionId;
            public string[] staffIds;
        }
    }

    [Serializable]
    public class OfficeModifiers
    {
        public float ProductivityMultiplier = 1f;
        public float MoraleBonus = 0f;
        public float RevenueMultiplier = 1f;
        public float StatsMultiplier = 1f;

        public void ApplyUpgrade(UpgradeDefinitionSO upgrade)
        {
            ProductivityMultiplier *= upgrade.productivityMultiplier;
            MoraleBonus += upgrade.moraleBonus;
            RevenueMultiplier *= upgrade.revenueMultiplier;
            StatsMultiplier *= upgrade.statsMultiplier;
        }
    }
}