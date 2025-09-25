using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FocusFounder.UI
{
    /// <summary>
    /// View component for the main company dashboard
    /// Binds to CompanyDashboardVM and updates UI elements
    /// </summary>
    public class CompanyDashboardView : MonoBehaviour
    {
        [Header("Currency Display")]
        [SerializeField] private TextMeshProUGUI cashText;
        [SerializeField] private TextMeshProUGUI researchText;
        [SerializeField] private TextMeshProUGUI reputationText;

        [Header("Focus Display")]
        [SerializeField] private TextMeshProUGUI focusTimeText;
        [SerializeField] private Image focusIndicator;
        [SerializeField] private Color focusedColor = Color.green;
        [SerializeField] private Color unfocusedColor = Color.red;

        [Header("Company Stats")]
        [SerializeField] private TextMeshProUGUI employeeCountText;
        [SerializeField] private TextMeshProUGUI officeCountText;

        [Header("Employee List")]
        [SerializeField] private Transform employeeListParent;
        [SerializeField] private GameObject employeeItemPrefab;

        private CompanyDashboardVM _viewModel;

        public void Initialize(CompanyDashboardVM viewModel)
        {
            _viewModel = viewModel;
        }

        private void OnEnable()
        {
            if (_viewModel != null)
            {
                _viewModel.Bind();
                BindToViewModel();
            }
        }

        private void OnDisable()
        {
            if (_viewModel != null)
            {
                UnbindFromViewModel();
                _viewModel.Unbind();
            }
        }

        private void BindToViewModel()
        {
            // Bind currency values
            _viewModel.Cash.OnValueChanged += OnCashChanged;
            _viewModel.Research.OnValueChanged += OnResearchChanged;
            _viewModel.Reputation.OnValueChanged += OnReputationChanged;

            // Bind focus state
            _viewModel.FocusTime.OnValueChanged += OnFocusTimeChanged;
            _viewModel.IsFocused.OnValueChanged += OnFocusStateChanged;

            // Bind company stats
            _viewModel.EmployeeCount.OnValueChanged += OnEmployeeCountChanged;
            _viewModel.OfficeCount.OnValueChanged += OnOfficeCountChanged;

            // Bind employee collection
            _viewModel.Employees.OnItemAdded += OnEmployeeAdded;
            _viewModel.Employees.OnItemRemoved += OnEmployeeRemoved;
            _viewModel.Employees.OnCollectionChanged += OnEmployeeCollectionChanged;

            // Initialize display with current values
            OnCashChanged(_viewModel.Cash.Value);
            OnResearchChanged(_viewModel.Research.Value);
            OnReputationChanged(_viewModel.Reputation.Value);
            OnFocusStateChanged(_viewModel.IsFocused.Value);
            OnEmployeeCountChanged(_viewModel.EmployeeCount.Value);
            OnOfficeCountChanged(_viewModel.OfficeCount.Value);
        }

        private void UnbindFromViewModel()
        {
            if (_viewModel == null) return;

            _viewModel.Cash.OnValueChanged -= OnCashChanged;
            _viewModel.Research.OnValueChanged -= OnResearchChanged;
            _viewModel.Reputation.OnValueChanged -= OnReputationChanged;
            _viewModel.FocusTime.OnValueChanged -= OnFocusTimeChanged;
            _viewModel.IsFocused.OnValueChanged -= OnFocusStateChanged;
            _viewModel.EmployeeCount.OnValueChanged -= OnEmployeeCountChanged;
            _viewModel.OfficeCount.OnValueChanged -= OnOfficeCountChanged;
            _viewModel.Employees.OnItemAdded -= OnEmployeeAdded;
            _viewModel.Employees.OnItemRemoved -= OnEmployeeRemoved;
            _viewModel.Employees.OnCollectionChanged -= OnEmployeeCollectionChanged;
        }

        private void OnCashChanged(float value)
        {
            if (cashText != null)
                cashText.text = $"${value:F0}";
        }

        private void OnResearchChanged(float value)
        {
            if (researchText != null)
                researchText.text = $"{value:F0} RP";
        }

        private void OnReputationChanged(float value)
        {
            if (reputationText != null)
                reputationText.text = $"{value:F0} REP";
        }

        private void OnFocusTimeChanged(System.TimeSpan time)
        {
            if (focusTimeText != null)
                focusTimeText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
        }

        private void OnFocusStateChanged(bool isFocused)
        {
            if (focusIndicator != null)
                focusIndicator.color = isFocused ? focusedColor : unfocusedColor;
        }

        private void OnEmployeeCountChanged(int count)
        {
            if (employeeCountText != null)
                employeeCountText.text = $"Employees: {count}";
        }

        private void OnOfficeCountChanged(int count)
        {
            if (officeCountText != null)
                officeCountText.text = $"Offices: {count}";
        }

        private void OnEmployeeAdded(Domain.Employee employee)
        {
            CreateEmployeeListItem(employee);
        }

        private void OnEmployeeRemoved(Domain.Employee employee)
        {
            // Find and destroy the corresponding UI element
            // Implementation would track employee UI items
        }

        private void OnEmployeeCollectionChanged()
        {
            // Rebuild entire employee list if needed
            RebuildEmployeeList();
        }

        private void CreateEmployeeListItem(Domain.Employee employee)
        {
            if (employeeItemPrefab != null && employeeListParent != null)
            {
                var item = Instantiate(employeeItemPrefab, employeeListParent);
                // Configure employee item UI with employee data
                var itemText = item.GetComponentInChildren<TextMeshProUGUI>();
                if (itemText != null)
                    itemText.text = employee.Archetype.displayName;
            }
        }

        private void RebuildEmployeeList()
        {
            // Clear existing items
            for (int i = employeeListParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(employeeListParent.GetChild(i).gameObject);
            }

            // Recreate items
            foreach (var employee in _viewModel.Employees)
            {
                CreateEmployeeListItem(employee);
            }
        }
    }
}