# 4. ScriptableObjects for data definitions
so_scripts = {}

# EmployeeArchetypeSO.cs
so_scripts["EmployeeArchetypeSO.cs"] = '''using UnityEngine;

namespace FocusFounder.Data
{
    using Domain;

    [CreateAssetMenu(fileName = "Employee_", menuName = "Focus Founder/Employee Archetype")]
    public class EmployeeArchetypeSO : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public string displayName;
        public string description;
        public Sprite portrait;

        [Header("Base Stats")]
        public EmployeeStats baseStats = new EmployeeStats(1f, 100f, 1f, 1f);
        
        [Header("Growth")]
        public AnimationCurve productivityGrowth = AnimationCurve.Linear(1f, 1f, 10f, 2f);
        public AnimationCurve efficiencyGrowth = AnimationCurve.Linear(1f, 1f, 10f, 1.5f);
        
        [Header("Task Preferences")]
        public string[] allowedTaskCategories;
        public float[] categoryMultipliers;

        [Header("Visual")]
        public string animationSetId;
        public Color characterColor = Color.white;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
                id = name.ToLower().Replace(" ", "_");
        }
    }
}'''

# TaskDefinitionSO.cs
so_scripts["TaskDefinitionSO.cs"] = '''using UnityEngine;

namespace FocusFounder.Data
{
    using Domain;

    public enum TaskCategory
    {
        Development,
        Marketing,
        Operations,
        Sales,
        Support,
        Research
    }

    [CreateAssetMenu(fileName = "Task_", menuName = "Focus Founder/Task Definition")]
    public class TaskDefinitionSO : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public string displayName;
        public string description;
        public TaskCategory category;

        [Header("Timing")]
        public float baseDuration = 30f; // seconds
        public float durationVariation = 0.2f; // ±20%

        [Header("Output")]
        public RewardBundle baseReward;
        public AnimationCurve difficultyMultiplier = AnimationCurve.Constant(1f, 10f, 1f);

        [Header("Requirements")]
        public string[] requiredSkills;
        public float minEmployeeLevel = 1f;

        [Header("Visual & Audio")]
        public Sprite taskIcon;
        public string completionVFX;
        public string completionSFX;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
                id = name.ToLower().Replace(" ", "_");
            
            baseDuration = Mathf.Max(1f, baseDuration);
        }

        public float GetDurationForLevel(float level)
        {
            var variation = Random.Range(-durationVariation, durationVariation);
            return baseDuration * (1f + variation) / Mathf.Sqrt(level);
        }

        public RewardBundle GetRewardForLevel(float level)
        {
            var multiplier = difficultyMultiplier.Evaluate(level);
            return baseReward * multiplier;
        }
    }
}'''

# OfficeDefinitionSO.cs
so_scripts["OfficeDefinitionSO.cs"] = '''using UnityEngine;

namespace FocusFounder.Data
{
    using Domain;

    [CreateAssetMenu(fileName = "Office_", menuName = "Focus Founder/Office Definition")]
    public class OfficeDefinitionSO : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public string displayName;
        public string description;

        [Header("Capacity")]
        public int maxStaff = 5;
        public Vector2Int gridSize = new Vector2Int(10, 10);

        [Header("Base Modifiers")]
        public float productivityMultiplier = 1f;
        public float moraleBonus = 0f;
        public float revenueMultiplier = 1f;

        [Header("Unlock Requirements")]
        public CostBundle unlockCost;
        public float requiredCompanyLevel = 1f;
        public string[] prerequisiteOfficeIds;

        [Header("Visual")]
        public Sprite backgroundSprite;
        public string themeId;
        public Vector2 cameraPosition;
        public float cameraSize = 5f;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
                id = name.ToLower().Replace(" ", "_");
            
            maxStaff = Mathf.Max(1, maxStaff);
            gridSize.x = Mathf.Max(5, gridSize.x);
            gridSize.y = Mathf.Max(5, gridSize.y);
        }
    }
}'''

# UpgradeDefinitionSO.cs
so_scripts["UpgradeDefinitionSO.cs"] = '''using UnityEngine;

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
}'''

# Save ScriptableObject scripts
import os
os.makedirs("Unity_Scripts/Data", exist_ok=True)
for name, content in so_scripts.items():
    with open(f"Unity_Scripts/Data/{name}", "w") as f:
        f.write(content)

print("ScriptableObject Scripts Created:")
for name in so_scripts.keys():
    print(f"- {name}")