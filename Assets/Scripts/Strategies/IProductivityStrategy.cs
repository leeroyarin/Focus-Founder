namespace FocusFounder.Strategies
{
    using Domain;

    /// <summary>
    /// Strategy for calculating employee productivity rates
    /// </summary>
    public interface IProductivityStrategy
    {
        float ComputeRate(Employee employee, Office office, TaskInstance task, GlobalModifiers globalMods);
    }
}