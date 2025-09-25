using System;
using UnityEngine;

namespace FocusFounder.Domain
{
    [Serializable]
    public struct RewardBundle
    {
        [SerializeField] public float cash;
        [SerializeField] public float research;
        [SerializeField] public float reputation;
        [SerializeField] public float experience;

        public RewardBundle(float cash = 0f, float research = 0f, float reputation = 0f, float experience = 0f)
        {
            this.cash = cash;
            this.research = research;
            this.reputation = reputation;
            this.experience = experience;
        }

        public static RewardBundle operator +(RewardBundle a, RewardBundle b)
        {
            return new RewardBundle(
                a.cash + b.cash,
                a.research + b.research,
                a.reputation + b.reputation,
                a.experience + b.experience
            );
        }

        public static RewardBundle operator *(RewardBundle bundle, float multiplier)
        {
            return new RewardBundle(
                bundle.cash * multiplier,
                bundle.research * multiplier,
                bundle.reputation * multiplier,
                bundle.experience * multiplier
            );
        }
    }
}