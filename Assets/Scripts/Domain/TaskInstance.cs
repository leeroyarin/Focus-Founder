using System;
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
}