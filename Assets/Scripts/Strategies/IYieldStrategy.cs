namespace FocusFounder.Strategies
{
    using Domain;

    /// <summary>
    /// Strategy for calculating task completion rewards
    /// </summary>
    public interface IYieldStrategy
    {
        RewardBundle ComputeYield(Employee employee, TaskInstance task, GlobalModifiers globalMods);
    }
}