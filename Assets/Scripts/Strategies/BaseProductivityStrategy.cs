using UnityEngine;

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
}