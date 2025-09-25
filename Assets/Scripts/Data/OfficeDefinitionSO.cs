using UnityEngine;

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
}