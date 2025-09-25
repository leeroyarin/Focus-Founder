using System;
using UnityEngine;

namespace FocusFounder.Domain
{
    [Serializable]
    public struct CostBundle
    {
        [SerializeField] public float cash;
        [SerializeField] public float research;
        [SerializeField] public float reputation;

        public CostBundle(float cash = 0f, float research = 0f, float reputation = 0f)
        {
            this.cash = cash;
            this.research = research;
            this.reputation = reputation;
        }

        public bool IsEmpty => cash <= 0f && research <= 0f && reputation <= 0f;
    }

    [Serializable]
    public struct CurrencyBalance
    {
        [SerializeField] public float cash;
        [SerializeField] public float research;
        [SerializeField] public float reputation;

        public CurrencyBalance(float cash = 0f, float research = 0f, float reputation = 0f)
        {
            this.cash = cash;
            this.research = research;
            this.reputation = reputation;
        }

        public bool CanAfford(CostBundle cost)
        {
            return cash >= cost.cash && 
                   research >= cost.research && 
                   reputation >= cost.reputation;
        }

        public CurrencyBalance Spend(CostBundle cost)
        {
            return new CurrencyBalance(
                cash - cost.cash,
                research - cost.research,
                reputation - cost.reputation
            );
        }

        public CurrencyBalance Add(RewardBundle reward)
        {
            return new CurrencyBalance(
                cash + reward.cash,
                research + reward.research,
                reputation + reward.reputation
            );
        }
    }
}