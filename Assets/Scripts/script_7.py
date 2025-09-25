# 6. Core Services
service_scripts = {}

# IEconomyService.cs
service_scripts["IEconomyService.cs"] = '''namespace FocusFounder.Services
{
    using Domain;

    /// <summary>
    /// Manages the game's economy - currencies, rewards, and spending
    /// </summary>
    public interface IEconomyService
    {
        CurrencyBalance GetBalances();
        void Add(RewardBundle rewards);
        bool TrySpend(CostBundle cost);
        bool CanAfford(CostBundle cost);
        
        // Events
        event System.Action<CurrencyBalance> OnBalanceChanged;
        event System.Action<RewardBundle> OnRewardReceived;
        event System.Action<CostBundle> OnCostPaid;
    }
}'''

# EconomyService.cs
service_scripts["EconomyService.cs"] = '''using UnityEngine;

namespace FocusFounder.Services
{
    using Core;
    using Domain;

    /// <summary>
    /// Manages all economic transactions and currency balances
    /// </summary>
    public class EconomyService : MonoBehaviour, IEconomyService, ISaveable
    {
        [SerializeField] private CurrencyBalance _startingBalance = new CurrencyBalance(100f, 0f, 0f);
        
        private CurrencyBalance _currentBalance;
        
        public event System.Action<CurrencyBalance> OnBalanceChanged;
        public event System.Action<RewardBundle> OnRewardReceived;  
        public event System.Action<CostBundle> OnCostPaid;

        public string SaveKey => "Economy";

        private void Awake()
        {
            _currentBalance = _startingBalance;
        }

        public CurrencyBalance GetBalances() => _currentBalance;

        public void Add(RewardBundle rewards)
        {
            _currentBalance = _currentBalance.Add(rewards);
            OnRewardReceived?.Invoke(rewards);
            OnBalanceChanged?.Invoke(_currentBalance);
        }

        public bool TrySpend(CostBundle cost)
        {
            if (CanAfford(cost))
            {
                _currentBalance = _currentBalance.Spend(cost);
                OnCostPaid?.Invoke(cost);
                OnBalanceChanged?.Invoke(_currentBalance);
                return true;
            }
            return false;
        }

        public bool CanAfford(CostBundle cost)
        {
            return _currentBalance.CanAfford(cost);
        }

        public object CaptureState()
        {
            return _currentBalance;
        }

        public void RestoreState(object state)
        {
            if (state is CurrencyBalance balance)
            {
                _currentBalance = balance;
                OnBalanceChanged?.Invoke(_currentBalance);
            }
        }
    }
}'''

# ITaskService.cs
service_scripts["ITaskService.cs"] = '''using System.Collections.Generic;

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
}'''

# TaskService.cs  
service_scripts["TaskService.cs"] = '''using System.Collections.Generic;
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
}'''

# Save Service scripts
import os
os.makedirs("Unity_Scripts/Services", exist_ok=True)
for name, content in service_scripts.items():
    with open(f"Unity_Scripts/Services/{name}", "w") as f:
        f.write(content)

print("Service Scripts Created:")
for name in service_scripts.keys():
    print(f"- {name}")