using System.Collections.Generic;

namespace FocusFounder.Services
{
    using Domain;
    using Data;

    /// <summary>
    /// Manages office operations, layouts, and unlocking
    /// </summary>
    public interface IOfficeService
    {
        List<Office> GetAllOffices();
        Office GetOffice(string id);
        bool TryUnlockOffice(OfficeDefinitionSO definition);
        Office GetOfficeForEmployee(Employee employee);
        void TickAllOffices(float deltaTime);

        // Events
        event System.Action<Office> OnOfficeUnlocked;
        event System.Action<Office, Employee> OnEmployeeMovedToOffice;
    }
}