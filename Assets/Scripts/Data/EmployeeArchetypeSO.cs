using UnityEngine;

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
}