using System.Collections.Generic;

namespace FocusFounder.Services
{
    using Domain;
    using Data;

    /// <summary>
    /// Manages employee hiring, assignment, and progression
    /// </summary>
    public interface IEmployeeService
    {
        List<Employee> GetAllEmployees();
        Employee GetEmployee(string id);
        bool TryHireEmployee(EmployeeArchetypeSO archetype, Office office);
        bool TryAssignToOffice(Employee employee, Office office);
        void TickAllEmployees(float deltaTime);

        // Events
        event System.Action<Employee> OnEmployeeHired;
        event System.Action<Employee, Office> OnEmployeeAssigned;
        event System.Action<Employee> OnEmployeeLevelUp;
    }
}