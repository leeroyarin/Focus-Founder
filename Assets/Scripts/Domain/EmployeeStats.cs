using System;
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
}