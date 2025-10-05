using UnityEngine;

namespace FocusFounder.Services
{
    using Core;
    using Domain;

    /// <summary>
    /// Manages all economic transactions and currency balances
    /// </summary>
    public class EconomyService : Singleton<EconomyService>, IEconomyService, ISaveable
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

        public void InitializeOnSaver()
        {
            Services.Get<SaveService>().RegisterSavableObjects(this);
        }
    }
}