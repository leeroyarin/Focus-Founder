using System;
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
}