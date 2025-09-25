using System.Collections.Generic;

namespace FocusFounder.Services
{
    using Domain;
    using Data;

    /// <summary>
    /// Manages task queues, routing, and assignment
    /// </summary>
    public interface ITaskService
    {
        void QueueTask(TaskDefinitionSO taskDef, Office office);
        TaskInstance GetNextTask(Office office, Employee employee);
        void CompleteTask(Employee employee, TaskInstance task);
        List<TaskInstance> GetQueuedTasks(Office office);
        void ClearQueue(Office office);

        // Events
        event System.Action<TaskInstance> OnTaskQueued;
        event System.Action<Employee, TaskInstance> OnTaskCompleted;
        event System.Action<Employee, TaskInstance> OnTaskStarted;
    }
}