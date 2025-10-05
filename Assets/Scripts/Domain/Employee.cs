using System;
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

        public void InitializeOnSaver()
        {
            Singleton<SaveService>.Instance.RegisterSavableObjects( this );
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
}