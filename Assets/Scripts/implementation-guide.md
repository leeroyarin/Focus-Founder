# Focus Founder Unity Implementation Guide

## Overview

This guide provides step-by-step instructions for implementing the Focus Founder game systems in Unity 6, based on the provided technical design document. The architecture follows SOLID principles and uses a service-oriented approach with dependency injection.

## Project Setup

### 1. Unity Project Configuration

1. **Create New Project:**
   - Unity 6 (2024.2+)
   - Use URP (Universal Render Pipeline) template
   - Platform: Mobile (iOS/Android primary)

2. **Required Packages:**
   ```
   - TextMeshPro
   - Addressables (com.unity.addressables)
   - Input System (com.unity.inputsystem) 
   - Newtonsoft Json (com.unity.nuget.newtonsoft-json)
   - Unity UI (com.unity.ugui)
   ```

3. **Project Structure:**
   ```
   Assets/
   ├── Scripts/
   │   ├── Core/
   │   ├── Focus/
   │   ├── Domain/
   │   ├── Data/
   │   ├── Services/
   │   ├── Strategies/
   │   ├── Animation/
   │   ├── UI/
   │   └── Management/
   ├── Data/
   │   ├── Employees/
   │   ├── Tasks/
   │   ├── Offices/
   │   ├── Upgrades/
   │   └── Themes/
   ├── Prefabs/
   ├── UI/
   ├── Art/
   └── Audio/
   ```

### 2. Script Implementation Order

**Phase 1: Core Infrastructure**
1. Copy all scripts from `Core/` folder
2. Copy all scripts from `Focus/` folder  
3. Copy all scripts from `Domain/` folder

**Phase 2: Data Layer**
1. Copy all scripts from `Data/` folder (ScriptableObjects)
2. Copy all scripts from `Strategies/` folder

**Phase 3: Services**
1. Copy all scripts from `Services/` folder
2. Resolve any compilation errors by implementing missing dependencies

**Phase 4: Presentation**
1. Copy all scripts from `Animation/` folder
2. Copy all scripts from `UI/` folder
3. Copy all scripts from `Management/` folder

## Scene Setup

### 1. Main Scene Structure

Create a single scene called "Main" with the following hierarchy:

```
Main
├── GameManager
├── Services
│   ├── FocusService
│   ├── SimulationClock  
│   ├── EconomyService
│   ├── EmployeeService
│   ├── OfficeService
│   └── TaskService
├── UI
│   ├── Canvas
│   │   ├── DashboardView
│   │   │   ├── Header (Currency, Focus Status)
│   │   │   ├── MainContent (Office View)
│   │   │   └── Footer (Navigation Tabs)
│   │   └── Modals (Dialogs, Popups)
│   └── DashboardViewModel
└── Office Views
    └── OfficeContainer (for office scenes)
```

### 2. GameManager Setup

1. Create empty GameObject named "GameManager"
2. Attach `GameManager.cs` script
3. In the Inspector, drag references to all service objects
4. Configure auto-save settings

### 3. Service Objects Setup

For each service:

1. Create empty GameObject with service name
2. Attach corresponding service script
3. Make them children of "Services" object
4. Configure serialized fields in Inspector

### 4. UI Setup

1. Create UI Canvas (Screen Space - Overlay)
2. Attach `CompanyDashboardView.cs` to main UI panel
3. Create separate GameObject for `CompanyDashboardVM.cs`
4. Connect ViewModel to View in Inspector
5. Design UI layout with TextMeshPro components

## ScriptableObject Creation

### 1. Employee Archetypes

1. **Create Folder:** `Assets/Data/Employees/`
2. **Create Assets:** Right-click → Create → Focus Founder → Employee Archetype
3. **Configure Sample Employees:**

```
Employee_Developer:
- displayName: "Software Developer"
- baseStats: Productivity(1.2), Morale(80), Efficiency(1.1), Quality(1.3)
- allowedTaskCategories: ["Development", "Research"]

Employee_Designer:
- displayName: "UI/UX Designer"  
- baseStats: Productivity(1.0), Morale(90), Efficiency(1.0), Quality(1.5)
- allowedTaskCategories: ["Development", "Design"]

Employee_Marketer:
- displayName: "Marketing Specialist"
- baseStats: Productivity(0.9), Morale(95), Efficiency(1.2), Quality(1.1)
- allowedTaskCategories: ["Marketing", "Sales"]
```

### 2. Task Definitions

1. **Create Folder:** `Assets/Data/Tasks/`
2. **Create Sample Tasks:**

```
Task_FeatureDevelopment:
- displayName: "Develop Feature"
- category: Development
- baseDuration: 45f
- baseReward: Cash(50), Research(10), Experience(15)

Task_BugFix:
- displayName: "Fix Bug"
- category: Development  
- baseDuration: 20f
- baseReward: Cash(25), Research(5), Experience(8)

Task_Marketing:
- displayName: "Marketing Campaign"
- category: Marketing
- baseDuration: 30f
- baseReward: Cash(40), Reputation(15), Experience(10)
```

### 3. Office Definitions

1. **Create Folder:** `Assets/Data/Offices/`
2. **Create Sample Offices:**

```
Office_Garage:
- displayName: "Garage"
- maxStaff: 3
- gridSize: (8, 6)
- unlockCost: Cash(0) // Starting office
- productivityMultiplier: 1.0f

Office_Coworking:
- displayName: "Co-working Space"
- maxStaff: 8
- gridSize: (12, 10)
- unlockCost: Cash(1000), Reputation(50)
- productivityMultiplier: 1.2f
```

### 4. Strategy Objects

1. **Create Folder:** `Assets/Data/Strategies/`
2. **Create:** Right-click → Create → Focus Founder → Strategies → Productivity
3. **Create:** Right-click → Create → Focus Founder → Strategies → Yield
4. Configure base multipliers and curves

## Animation Setup

### 1. Animator Controllers

1. **Create Folder:** `Assets/Animation/Controllers/`
2. **Create Animator Controller:** "EmployeeAnimator"
3. **Add Parameters:**
   - WorkIntensity (Float)
   - Trigger parameters: Work, Idle, Celebrate, FocusPulse

4. **Create States:**
   - Idle (default)
   - Working (looping)
   - Celebrating (one-shot)
   - Focus Pulse (UI effect)

5. **Create Transitions:**
   - Idle ↔ Working (Work/Idle triggers)
   - Any State → Celebrating (Celebrate trigger)
   - Any State → Focus Pulse (FocusPulse trigger)

### 2. Employee Prefab Setup

1. **Create Employee Prefab:**
   ```
   Employee
   ├── Visual
   │   ├── SpriteRenderer (character art)
   │   └── Animator (attach EmployeeAnimator)
   ├── WorkEffectAnchor (empty transform)
   └── Scripts
       ├── EmployeeView
       └── AnimatorAdapter
   ```

2. **Configure Components:**
   - Link AnimatorAdapter to Animator
   - Link EmployeeView to AnimatorAdapter
   - Set up sprite and colors

## Focus System Configuration

### 1. Focus Service Setup

1. Attach `FocusService.cs` to service object
2. **Configure Settings:**
   - Debounce Sec: 1.0f
   - Min Session Sec: 3.0f

3. **Test Focus Detection:**
   - Build to device
   - Test app switching behavior
   - Monitor focus state in debug UI

### 2. Anti-Cheese Measures

1. **Implement Debouncing:**
   - Short focus switches ignored
   - Only sustained focus counts

2. **Session Validation:**
   - Minimum session duration required
   - Grace period for accidental switches

## Economy System Setup

### 1. Starting Balance Configuration

In `EconomyService.cs`:
```csharp
[SerializeField] private CurrencyBalance _startingBalance = new CurrencyBalance(500f, 0f, 10f);
```

### 2. Currency Display

1. Create UI elements for each currency
2. Bind to ViewModel properties
3. Format currency display (e.g., "$1,234", "150 RP")

### 3. Transaction Testing

1. Create debug buttons for adding/spending currency
2. Test overflow/underflow scenarios
3. Verify event system works correctly

## Task System Implementation

### 1. Task Queue Setup

1. **Configure Task Service:**
   - Link to default yield strategy
   - Set up task routing policies

2. **Create Task UI:**
   - Queue display panel
   - Task progress bars
   - Completion animations

3. **Test Task Flow:**
   - Queue tasks manually
   - Verify employee assignment
   - Check reward calculation

### 2. Auto-Task Generation

1. **Implement Task Factory:**
   ```csharp
   public void GenerateTasksForOffice(Office office)
   {
       // Create mix of task types based on office level
       // Queue appropriate number of tasks
   }
   ```

## UI Data Binding Implementation

### 1. Observable Properties

1. **Test Observable System:**
   ```csharp
   var testProperty = new ObservableProperty<float>(100f);
   testProperty.OnValueChanged += value => Debug.Log($"Value: {value}");
   testProperty.Value = 200f; // Should trigger event
   ```

### 2. ViewModel Binding

1. **Dashboard Binding:**
   - Currency values update in real-time
   - Focus timer displays correctly
   - Employee list updates on hire/fire

2. **Event Flow Testing:**
   ```
   Service Event → ViewModel Update → UI Refresh
   ```

## Save System Implementation

### 1. Save Data Structure

```csharp
[Serializable]
public class GameSaveData
{
    public object economyState;
    public object[] employeeStates;
    public object[] officeStates;
    public DateTime saveTime;
    public string version;
}
```

### 2. Save/Load Process

1. **Save Game:**
   - Iterate through all ISaveable services
   - Capture state objects
   - Serialize to JSON
   - Write to persistent storage

2. **Load Game:**
   - Read save file
   - Deserialize JSON
   - Restore service states
   - Rebuild object relationships

## Testing and Validation

### 1. Core Systems Testing

**Focus System:**
- [ ] App focus detection works on device
- [ ] Debouncing prevents cheese
- [ ] Session timing is accurate
- [ ] Events fire correctly

**Economy System:**
- [ ] Currency math is correct
- [ ] Transactions work properly
- [ ] UI updates in real-time
- [ ] Save/load preserves balances

**Employee System:**
- [ ] Hiring deducts correct cost
- [ ] Leveling system works
- [ ] Morale affects productivity
- [ ] Animation states sync correctly

**Task System:**
- [ ] Tasks queue properly
- [ ] Assignment algorithm works
- [ ] Rewards calculate correctly
- [ ] Completion triggers events

### 2. Integration Testing

**Service Communication:**
- [ ] EventBus routes messages correctly
- [ ] Services initialize in proper order
- [ ] Dependencies resolve correctly
- [ ] No circular dependencies

**UI Data Flow:**
- [ ] ViewModels update from services
- [ ] Views refresh from ViewModels
- [ ] User actions trigger service calls
- [ ] Observable properties work correctly

### 3. Performance Testing

**Mobile Performance:**
- [ ] 60 FPS maintained on target devices
- [ ] Memory usage within limits
- [ ] Battery drain is reasonable
- [ ] Touch responsiveness is good

## Advanced Features Implementation

### 1. Office Customization System

1. **Grid System:**
   - Implement snap-to-grid placement
   - Collision detection for decorations
   - Save/load layout data

2. **Decoration Effects:**
   - Apply stat bonuses from decorations
   - Visual feedback for bonus areas
   - Theme consistency checking

### 2. Progression System

1. **Company Levels:**
   - XP accumulation from tasks
   - Level-up rewards
   - Unlock conditions for content

2. **Employee Development:**
   - Skill trees for employees
   - Specialization paths
   - Training systems

### 3. Analytics Integration

1. **Event Tracking:**
   - Focus session metrics
   - Progression milestones
   - Retention indicators

2. **Balance Tuning:**
   - Remote config for rates
   - A/B testing framework
   - Performance dashboards

## Troubleshooting Common Issues

### 1. Focus Detection Problems

**Issue:** Focus events not firing
**Solution:** 
- Check platform-specific focus behavior
- Verify OnApplicationFocus/OnApplicationPause implementation
- Test on actual device, not editor

**Issue:** False focus triggers
**Solution:**
- Increase debounce timer
- Implement better state validation
- Add logging to debug trigger sequence

### 2. Performance Issues

**Issue:** Frame rate drops
**Solution:**
- Profile with Unity Profiler
- Optimize UI update frequency
- Reduce garbage collection

**Issue:** Memory leaks
**Solution:**
- Properly dispose event subscriptions
- Clear object pools regularly
- Monitor reference cycles

### 3. Save System Problems

**Issue:** Save data corruption
**Solution:**
- Add save data validation
- Implement backup save files
- Version save data format

**Issue:** Performance during save
**Solution:**
- Implement async saving
- Reduce save frequency
- Compress save data

## Deployment Considerations

### 1. Platform-Specific Setup

**iOS:**
- Configure app focus behavior
- Test background app refresh
- Handle app lifecycle properly

**Android:**
- Test various Android versions
- Handle different multitasking behaviors
- Test battery optimization settings

### 2. Content Delivery

**Addressables:**
- Set up addressable groups
- Configure remote content delivery
- Test content updates

**Localization:**
- Set up string tables
- Test different languages
- Handle text overflow

### 3. Analytics and Monitoring

**Crash Reporting:**
- Integrate crash reporting SDK
- Set up alerting
- Monitor key metrics

**User Engagement:**
- Track focus session duration
- Monitor retention rates
- A/B test gameplay features

## Maintenance and Updates

### 1. Content Updates

- New employee types
- Additional task categories
- Office themes and decorations
- Seasonal events

### 2. Balance Updates

- Adjust progression curves
- Tune reward values
- Modify focus requirements
- Update cost scaling

### 3. Feature Expansion

- Multiplayer elements
- Social features
- Achievement system
- Premium content

---

## Conclusion

This implementation guide provides a comprehensive roadmap for building Focus Founder in Unity 6. The modular architecture allows for iterative development and easy content expansion. Key success factors:

1. **Start Simple:** Implement core loop first
2. **Test Early:** Focus detection is critical
3. **Iterate Quickly:** Use data-driven approach
4. **Monitor Performance:** Mobile optimization is essential
5. **Plan for Growth:** Architecture supports expansion

The system is designed to be extensible and maintainable, following SOLID principles and using proven Unity patterns. Regular testing on target devices and iterative refinement will ensure a polished final product.