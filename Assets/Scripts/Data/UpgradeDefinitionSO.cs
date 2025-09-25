using UnityEngine;

namespace FocusFounder.Data
{
    using Domain;

    public enum UpgradeTarget
    {
        Global,
        Office,
        Employee,
        Task
    }

    [CreateAssetMenu(fileName = "Upgrade_", menuName = "Focus Founder/Upgrade Definition")]
    public class UpgradeDefinitionSO : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public string displayName;
        public string description;
        public Sprite icon;

        [Header("Target")]
        public UpgradeTarget target;
        public string targetId; // specific office/employee type, or empty for all

        [Header("Effects")]
        public float productivityMultiplier = 1f;
        public float moraleBonus = 0f;
        public float revenueMultiplier = 1f;
        public float statsMultiplier = 1f;
        public float durationReduction = 0f; // 0-1, reduces task time

        [Header("Cost & Requirements")]
        public CostBundle cost;
        public float requiredLevel = 1f;
        public string[] prerequisiteUpgrades;

        [Header("Progression")]
        public bool isRepeatable = false;
        public int maxLevel = 1;
        public AnimationCurve costScaling = AnimationCurve.Linear(1f, 1f, 10f, 5f);

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
                id = name.ToLower().Replace(" ", "_");

            productivityMultiplier = Mathf.Max(0.1f, productivityMultiplier);
            revenueMultiplier = Mathf.Max(0.1f, revenueMultiplier);
            durationReduction = Mathf.Clamp01(durationReduction);
            maxLevel = Mathf.Max(1, maxLevel);
        }

        public CostBundle GetCostForLevel(int level)
        {
            if (!isRepeatable || level <= 1)
                return cost;

            var multiplier = costScaling.Evaluate(level);
            return new CostBundle(
                cost.cash * multiplier,
                cost.research * multiplier,
                cost.reputation * multiplier
            );
        }
    }
}