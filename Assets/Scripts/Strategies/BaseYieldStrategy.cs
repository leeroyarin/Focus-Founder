using UnityEngine;

namespace FocusFounder.Strategies
{
    using Domain;
    using Data;

    [CreateAssetMenu(fileName = "YieldStrategy_", menuName = "Focus Founder/Strategies/Yield")]
    public class BaseYieldStrategy : ScriptableObject, IYieldStrategy
    {
        [Header("Quality Scaling")]
        [SerializeField] private float qualityMultiplier = 1f;
        [SerializeField] private AnimationCurve qualityCurve = AnimationCurve.Linear(0f, 0.5f, 100f, 1.5f);

        public RewardBundle ComputeYield(Employee employee, TaskInstance task, GlobalModifiers globalMods)
        {
            var baseReward = task.Definition.GetRewardForLevel(employee.Level);

            // Quality impact
            var qualityMultiplier = qualityCurve.Evaluate(employee.Stats.quality);

            // Global revenue modifier
            var globalMultiplier = globalMods.RevenueMultiplier;

            // Apply multipliers
            var finalReward = baseReward * qualityMultiplier * globalMultiplier;

            // Add experience based on task completion
            finalReward.experience += 10f + (task.Definition.baseDuration / 10f);

            return finalReward;
        }
    }
}