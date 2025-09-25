# Continue with main domain entities

# TaskInstance.cs
domain_scripts["TaskInstance.cs"] = '''using System;
using UnityEngine;

namespace FocusFounder.Domain
{
    using Core;
    using Data;

    [Serializable]
    public sealed class TaskInstance : ISaveable
    {
        [SerializeField] private string _id;
        [SerializeField] private float _remaining;
        [SerializeField] private float _totalDuration;
        
        public string Id => _id;
        public TaskDefinitionSO Definition { get; private set; }
        public float Remaining => _remaining;
        public float TotalDuration => _totalDuration;
        public float Progress => 1f - (_remaining / _totalDuration);
        public bool IsComplete => _remaining <= 0f;

        public string SaveKey => $"Task_{_id}";

        public TaskInstance(TaskDefinitionSO definition)
        {
            _id = Guid.NewGuid().ToString();
            Definition = definition;
            _totalDuration = definition.baseDuration;
            _remaining = _totalDuration;
        }

        public void Advance(float deltaTime)
        {
            if (_remaining > 0f)
            {
                _remaining = Mathf.Max(0f, _remaining - deltaTime);
            }
        }

        public object CaptureState()
        {
            return new TaskSaveData
            {
                id = _id,
                definitionId = Definition.id,
                remaining = _remaining,
                totalDuration = _totalDuration
            };
        }

        public void RestoreState(object state)
        {
            if (state is TaskSaveData data)
            {
                _id = data.id;
                _remaining = data.remaining;
                _totalDuration = data.totalDuration;
                // Definition should be resolved by ContentService
            }
        }

        [Serializable]
        private struct TaskSaveData
        {
            public string id;
            public string definitionId;
            public float remaining;
            public float totalDuration;
        }
    }
}'''

# Employee.cs
domain_scripts["Employee.cs"] = '''using System;
using UnityEngine;

namespace FocusFounder.Domain
{
    using Core;
    using Data;
    using Services;

    [Serializable]
    public sealed class Employee : ISaveable
    {
        [SerializeField] private string _id;
        [SerializeField] private float _level = 1f;
        [SerializeField] private float _morale = 100f;
        [SerializeField] private float _experience = 0f;
        
        public string Id => _id;
        public EmployeeArchetypeSO Archetype { get; private set; }
        public float Level => _level;
        public float Morale => _morale;
        public float Experience => _experience;
        public TaskInstance CurrentTask { get; private set; }
        public EmployeeState State { get; private set; } = EmployeeState.Idle;
        
        public EmployeeStats Stats => CalculateStats();

        public string SaveKey => $"Employee_{_id}";

        public Employee(EmployeeArchetypeSO archetype)
        {
            _id = Guid.NewGuid().ToString();
            Archetype = archetype;
            _level = 1f;
            _morale = 100f;
            _experience = 0f;
        }

        public void AssignTask(TaskInstance task)
        {
            CurrentTask = task;
            State = EmployeeState.Working;
        }

        public void CompleteTask()
        {
            CurrentTask = null;
            State = EmployeeState.Celebrating;
            GainExperience(10f); // Base XP gain
        }

        public void SetIdle()
        {
            CurrentTask = null;
            State = EmployeeState.Idle;
        }

        public void Tick(float deltaTime, ISimulationClock clock, IEconomyService economy)
        {
            if (CurrentTask != null && State == EmployeeState.Working)
            {
                CurrentTask.Advance(deltaTime * Stats.productivity);
                
                if (CurrentTask.IsComplete)
                {
                    CompleteTask();
                }
            }
            
            // Slowly recover morale over time
            if (_morale < 100f)
            {
                _morale = Mathf.Min(100f, _morale + deltaTime * 2f);
            }
        }

        public void GainExperience(float amount)
        {
            _experience += amount;
            
            // Level up logic
            float nextLevelXP = _level * 100f;
            if (_experience >= nextLevelXP)
            {
                _level++;
                _experience -= nextLevelXP;
            }
        }

        public void ModifyMorale(float delta)
        {
            _morale = Mathf.Clamp(_morale + delta, 0f, 100f);
        }

        private EmployeeStats CalculateStats()
        {
            var baseStats = Archetype.baseStats;
            var levelMultiplier = 1f + (_level - 1f) * 0.1f; // 10% per level
            var moraleMultiplier = _morale / 100f; // 0-1 based on morale
            
            return new EmployeeStats(
                baseStats.productivity * levelMultiplier * moraleMultiplier,
                _morale,
                baseStats.efficiency * levelMultiplier,
                baseStats.quality * levelMultiplier
            );
        }

        public object CaptureState()
        {
            return new EmployeeSaveData
            {
                id = _id,
                archetypeId = Archetype.id,
                level = _level,
                morale = _morale,
                experience = _experience,
                currentTaskId = CurrentTask?.Id,
                state = State
            };
        }

        public void RestoreState(object state)
        {
            if (state is EmployeeSaveData data)
            {
                _id = data.id;
                _level = data.level;
                _morale = data.morale;
                _experience = data.experience;
                State = data.state;
                // Archetype and CurrentTask should be resolved by services
            }
        }

        [Serializable]
        private struct EmployeeSaveData
        {
            public string id;
            public string archetypeId;
            public float level;
            public float morale;
            public float experience;
            public string currentTaskId;
            public EmployeeState state;
        }
    }
}'''

# Office.cs
domain_scripts["Office.cs"] = '''using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FocusFounder.Domain
{
    using Core;
    using Data;

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
}'''

# Save the additional domain scripts
for name, content in domain_scripts.items():
    with open(f"Unity_Scripts/Domain/{name}", "w") as f:
        f.write(content)

print("Additional Domain Scripts Created:")
for name in ["TaskInstance.cs", "Employee.cs", "Office.cs"]:
    print(f"- {name}")