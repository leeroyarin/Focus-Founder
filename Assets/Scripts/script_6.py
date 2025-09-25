# 5. Strategy Interfaces and Implementations
strategy_scripts = {}

# IProductivityStrategy.cs
strategy_scripts["IProductivityStrategy.cs"] = '''namespace FocusFounder.Strategies
{
    using Domain;

    /// <summary>
    /// Strategy for calculating employee productivity rates
    /// </summary>
    public interface IProductivityStrategy
    {
        float ComputeRate(Employee employee, Office office, TaskInstance task, GlobalModifiers globalMods);
    }
}'''

# IYieldStrategy.cs
strategy_scripts["IYieldStrategy.cs"] = '''namespace FocusFounder.Strategies
{
    using Domain;

    /// <summary>
    /// Strategy for calculating task completion rewards
    /// </summary>
    public interface IYieldStrategy
    {
        RewardBundle ComputeYield(Employee employee, TaskInstance task, GlobalModifiers globalMods);
    }
}'''

# BaseProductivityStrategy.cs
strategy_scripts["BaseProductivityStrategy.cs"] = '''using UnityEngine;

namespace FocusFounder.Strategies
{
    using Domain;

    [CreateAssetMenu(fileName = "ProductivityStrategy_", menuName = "Focus Founder/Strategies/Productivity")]
    public class BaseProductivityStrategy : ScriptableObject, IProductivityStrategy
    {
        [Header("Base Multipliers")]
        [SerializeField] private float baseMoraleMultiplier = 1f;
        [SerializeField] private float levelScaling = 0.1f;
        [SerializeField] private float focusBonus = 0.2f;

        public float ComputeRate(Employee employee, Office office, TaskInstance task, GlobalModifiers globalMods)
        {
            var baseRate = employee.Stats.productivity;
            
            // Morale impact
            var moraleMultiplier = Mathf.Lerp(0.5f, 1.5f, employee.Morale / 100f);
            
            // Level scaling
            var levelMultiplier = 1f + (employee.Level - 1f) * levelScaling;
            
            // Office modifiers
            var officeMultiplier = office.Modifiers.ProductivityMultiplier;
            
            // Global modifiers
            var globalMultiplier = globalMods.ProductivityMultiplier;
            
            // Focus bonus (applied when app is focused)
            var focusMultiplier = 1f + focusBonus;
            
            return baseRate * moraleMultiplier * levelMultiplier * officeMultiplier * globalMultiplier * focusMultiplier;
        }
    }
}'''

# BaseYieldStrategy.cs
strategy_scripts["BaseYieldStrategy.cs"] = '''using UnityEngine;

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
}'''

# Save Strategy scripts
import os
os.makedirs("Unity_Scripts/Strategies", exist_ok=True)
for name, content in strategy_scripts.items():
    with open(f"Unity_Scripts/Strategies/{name}", "w") as f:
        f.write(content)

print("Strategy Scripts Created:")
for name in strategy_scripts.keys():
    print(f"- {name}")