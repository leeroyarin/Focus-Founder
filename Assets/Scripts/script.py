# Let's organize the scripts we need to create based on the technical document
scripts_to_create = {
    "Core Infrastructure": [
        "IEvent.cs",
        "IEventBus.cs", 
        "EventBus.cs",
        "ITimeProvider.cs",
        "ISimulationClock.cs",
        "SimulationClock.cs",
        "ISaveable.cs"
    ],
    "Focus System": [
        "FocusEvents.cs",
        "IFocusService.cs",
        "FocusService.cs"
    ],
    "Domain Models": [
        "Employee.cs",
        "Office.cs", 
        "TaskInstance.cs",
        "RewardBundle.cs",
        "CostBundle.cs",
        "EmployeeStats.cs",
        "OfficeLayout.cs",
        "GlobalModifiers.cs"
    ],
    "ScriptableObjects": [
        "EmployeeArchetypeSO.cs",
        "TaskDefinitionSO.cs",
        "OfficeDefinitionSO.cs",
        "UpgradeDefinitionSO.cs",
        "ProjectDefinitionSO.cs",
        "ThemeDefinitionSO.cs",
        "CurveSetSO.cs",
        "AnimationSetSO.cs"
    ],
    "Services": [
        "IEconomyService.cs",
        "EconomyService.cs",
        "IEmployeeService.cs",
        "EmployeeService.cs",
        "IOfficeService.cs",
        "OfficeService.cs",
        "ITaskService.cs",
        "TaskService.cs",
        "IUpgradeService.cs",
        "UpgradeService.cs",
        "ICustomizationService.cs",
        "CustomizationService.cs",
        "ISaveService.cs",
        "SaveService.cs",
        "IAnalyticsService.cs",
        "AnalyticsService.cs",
        "IContentService.cs",
        "ContentService.cs"
    ],
    "Strategies": [
        "IProductivityStrategy.cs",
        "IYieldStrategy.cs",
        "BaseProductivityStrategy.cs",
        "BaseYieldStrategy.cs"
    ],
    "Animation System": [
        "IAnimPlayable.cs",
        "IAnimationBridge.cs",
        "AnimatorAdapter.cs",
        "EmployeeView.cs"
    ],
    "UI System": [
        "IViewModel.cs",
        "CompanyDashboardVM.cs",
        "CompanyDashboardView.cs",
        "ObservableProperty.cs",
        "ObservableCollection.cs"
    ],
    "Game Management": [
        "GameManager.cs",
        "ServiceLocator.cs"
    ]
}

total_scripts = sum(len(scripts) for scripts in scripts_to_create.values())
print(f"Total scripts to create: {total_scripts}")

for category, scripts in scripts_to_create.items():
    print(f"\n{category}: {len(scripts)} scripts")
    for script in scripts:
        print(f"  - {script}")