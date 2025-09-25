namespace FocusFounder.Services
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
}