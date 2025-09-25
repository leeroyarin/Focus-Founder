using UnityEngine;

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
}