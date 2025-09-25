using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FocusFounder.Services
{
    using Core;
    using Domain;
    using Data;
    using Strategies;

    /// <summary>
    /// Manages task creation, queuing, and assignment to employees
    /// </summary>
    public class TaskService : MonoBehaviour, ITaskService
    {
        [SerializeField] private BaseYieldStrategy defaultYieldStrategy;

        private Dictionary<string, Queue<TaskInstance>> _officeQueues = new();
        private IEconomyService _economyService;
        private IEventBus _eventBus;

        public event System.Action<TaskInstance> OnTaskQueued;
        public event System.Action<Employee, TaskInstance> OnTaskCompleted;
        public event System.Action<Employee, TaskInstance> OnTaskStarted;

        public void Initialize(IEconomyService economyService, IEventBus eventBus)
        {
            _economyService = economyService;
            _eventBus = eventBus;
        }

        public void QueueTask(TaskDefinitionSO taskDef, Office office)
        {
            var task = new TaskInstance(taskDef);

            if (!_officeQueues.ContainsKey(office.Id))
                _officeQueues[office.Id] = new Queue<TaskInstance>();

            _officeQueues[office.Id].Enqueue(task);
            OnTaskQueued?.Invoke(task);
        }

        public TaskInstance GetNextTask(Office office, Employee employee)
        {
            if (!_officeQueues.ContainsKey(office.Id))
                return null;

            var queue = _officeQueues[office.Id];
            if (queue.Count == 0)
                return null;

            // Simple FIFO for now - could implement priority/routing strategies
            var task = queue.Dequeue();
            OnTaskStarted?.Invoke(employee, task);
            return task;
        }

        public void CompleteTask(Employee employee, TaskInstance task)
        {
            // Calculate and award rewards
            var globalMods = new GlobalModifiers(); // Would be injected
            var reward = defaultYieldStrategy.ComputeYield(employee, task, globalMods);

            _economyService.Add(reward);
            OnTaskCompleted?.Invoke(employee, task);

            // Auto-queue another task of the same type for continuous work
            QueueTask(task.Definition, GetOfficeForEmployee(employee));
        }

        public List<TaskInstance> GetQueuedTasks(Office office)
        {
            if (!_officeQueues.ContainsKey(office.Id))
                return new List<TaskInstance>();

            return _officeQueues[office.Id].ToList();
        }

        public void ClearQueue(Office office)
        {
            if (_officeQueues.ContainsKey(office.Id))
                _officeQueues[office.Id].Clear();
        }

        private Office GetOfficeForEmployee(Employee employee)
        {
            // This would need to be resolved through OfficeService
            // For now, return null - proper implementation would track employee-office relationships
            return null;
        }
    }
}