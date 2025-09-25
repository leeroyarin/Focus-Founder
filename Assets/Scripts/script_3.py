# 3. Domain Models
domain_scripts = {}

# RewardBundle.cs
domain_scripts["RewardBundle.cs"] = '''using System;
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
}'''

# CostBundle.cs
domain_scripts["CostBundle.cs"] = '''using System;
using UnityEngine;

namespace FocusFounder.Domain
{
    [Serializable]
    public struct CostBundle
    {
        [SerializeField] public float cash;
        [SerializeField] public float research;
        [SerializeField] public float reputation;

        public CostBundle(float cash = 0f, float research = 0f, float reputation = 0f)
        {
            this.cash = cash;
            this.research = research;
            this.reputation = reputation;
        }

        public bool IsEmpty => cash <= 0f && research <= 0f && reputation <= 0f;
    }

    [Serializable]
    public struct CurrencyBalance
    {
        [SerializeField] public float cash;
        [SerializeField] public float research;
        [SerializeField] public float reputation;

        public CurrencyBalance(float cash = 0f, float research = 0f, float reputation = 0f)
        {
            this.cash = cash;
            this.research = research;
            this.reputation = reputation;
        }

        public bool CanAfford(CostBundle cost)
        {
            return cash >= cost.cash && 
                   research >= cost.research && 
                   reputation >= cost.reputation;
        }

        public CurrencyBalance Spend(CostBundle cost)
        {
            return new CurrencyBalance(
                cash - cost.cash,
                research - cost.research,
                reputation - cost.reputation
            );
        }

        public CurrencyBalance Add(RewardBundle reward)
        {
            return new CurrencyBalance(
                cash + reward.cash,
                research + reward.research,
                reputation + reward.reputation
            );
        }
    }
}'''

# EmployeeStats.cs
domain_scripts["EmployeeStats.cs"] = '''using System;
using UnityEngine;

namespace FocusFounder.Domain
{
    [Serializable]
    public struct EmployeeStats
    {
        [SerializeField] public float productivity;
        [SerializeField] public float morale;
        [SerializeField] public float efficiency;
        [SerializeField] public float quality;

        public EmployeeStats(float productivity, float morale, float efficiency, float quality)
        {
            this.productivity = productivity;
            this.morale = morale;
            this.efficiency = efficiency;
            this.quality = quality;
        }

        public static EmployeeStats operator +(EmployeeStats a, EmployeeStats b)
        {
            return new EmployeeStats(
                a.productivity + b.productivity,
                a.morale + b.morale,
                a.efficiency + b.efficiency,
                a.quality + b.quality
            );
        }

        public static EmployeeStats operator *(EmployeeStats stats, float multiplier)
        {
            return new EmployeeStats(
                stats.productivity * multiplier,
                stats.morale * multiplier,
                stats.efficiency * multiplier,
                stats.quality * multiplier
            );
        }
    }

    public enum EmployeeState
    {
        Idle,
        Working,
        Celebrating,
        Break
    }
}'''

# GlobalModifiers.cs
domain_scripts["GlobalModifiers.cs"] = '''using System.Collections.Generic;
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
}'''

# OfficeLayout.cs
domain_scripts["OfficeLayout.cs"] = '''using System;
using System.Collections.Generic;
using UnityEngine;

namespace FocusFounder.Domain
{
    [Serializable]
    public struct GridPosition
    {
        public int x;
        public int y;

        public GridPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition pos && pos.x == x && pos.y == y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }

    [Serializable]
    public class DecorationItem
    {
        public string itemId;
        public GridPosition position;
        public Vector2Int size;
        public EmployeeStats statBonus;

        public DecorationItem(string itemId, GridPosition position, Vector2Int size, EmployeeStats statBonus = default)
        {
            this.itemId = itemId;
            this.position = position;
            this.size = size;
            this.statBonus = statBonus;
        }
    }

    public class OfficeLayout
    {
        public Vector2Int GridSize { get; private set; }
        public Dictionary<GridPosition, DecorationItem> Decorations { get; private set; }

        public OfficeLayout(Vector2Int gridSize)
        {
            GridSize = gridSize;
            Decorations = new Dictionary<GridPosition, DecorationItem>();
        }

        public bool TryPlaceItem(DecorationItem item)
        {
            if (CanPlaceItem(item))
            {
                for (int x = 0; x < item.size.x; x++)
                {
                    for (int y = 0; y < item.size.y; y++)
                    {
                        var pos = new GridPosition(item.position.x + x, item.position.y + y);
                        Decorations[pos] = item;
                    }
                }
                return true;
            }
            return false;
        }

        public bool TryRemoveItem(GridPosition position)
        {
            if (Decorations.TryGetValue(position, out var item))
            {
                for (int x = 0; x < item.size.x; x++)
                {
                    for (int y = 0; y < item.size.y; y++)
                    {
                        var pos = new GridPosition(item.position.x + x, item.position.y + y);
                        Decorations.Remove(pos);
                    }
                }
                return true;
            }
            return false;
        }

        private bool CanPlaceItem(DecorationItem item)
        {
            for (int x = 0; x < item.size.x; x++)
            {
                for (int y = 0; y < item.size.y; y++)
                {
                    var pos = new GridPosition(item.position.x + x, item.position.y + y);
                    if (pos.x >= GridSize.x || pos.y >= GridSize.y || Decorations.ContainsKey(pos))
                        return false;
                }
            }
            return true;
        }
    }
}'''

# Save Domain scripts
import os
os.makedirs("Unity_Scripts/Domain", exist_ok=True)
for name, content in domain_scripts.items():
    with open(f"Unity_Scripts/Domain/{name}", "w") as f:
        f.write(content)

print("Domain Model Scripts Created:")
for name in domain_scripts.keys():
    print(f"- {name}")