using System.Collections.Generic;
using System.Linq;

namespace FocusFounder.Domain
{
    public class GlobalModifiers
    {
        public float ProductivityMultiplier => _productivityMultipliers.Aggregate(1f, (acc, val) => acc * val);
        public float MoraleMultiplier => _moraleMultipliers.Aggregate(1f, (acc, val) => acc * val);
        public float RevenueMultiplier => _revenueMultipliers.Aggregate(1f, (acc, val) => acc * val);

        private readonly List<float> _productivityMultipliers = new();
        private readonly List<float> _moraleMultipliers = new();
        private readonly List<float> _revenueMultipliers = new();

        public void AddProductivityMultiplier(float multiplier) => _productivityMultipliers.Add(multiplier);
        public void RemoveProductivityMultiplier(float multiplier) => _productivityMultipliers.Remove(multiplier);

        public void AddMoraleMultiplier(float multiplier) => _moraleMultipliers.Add(multiplier);
        public void RemoveMoraleMultiplier(float multiplier) => _moraleMultipliers.Remove(multiplier);

        public void AddRevenueMultiplier(float multiplier) => _revenueMultipliers.Add(multiplier);
        public void RemoveRevenueMultiplier(float multiplier) => _revenueMultipliers.Remove(multiplier);

        public void Clear()
        {
            _productivityMultipliers.Clear();
            _moraleMultipliers.Clear();
            _revenueMultipliers.Clear();
        }
    }
}