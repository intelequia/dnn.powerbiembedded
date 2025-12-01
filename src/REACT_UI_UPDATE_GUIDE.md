# Step-by-Step React UI Update Guide

## Summary
This guide provides the exact changes needed to update the React UI to work with the decoupled capacity settings architecture.

## Phase 1: Core Component Files (Already Created)

### ? Files Created:
1. `DotNetNuke.PowerBI\PBIEmbedded.Web\src\components\capacitySettings\index.jsx`
2. `DotNetNuke.PowerBI\PBIEmbedded.Web\src\components\capacitySettings\capacitySettings.jsx`
3. `DotNetNuke.PowerBI\PBIEmbedded.Web\src\components\capacitySettings\capacityEditor.jsx`
4. `DotNetNuke.PowerBI\PBIEmbedded.Web\src\components\capacitySettings\capacityRow.jsx`
5. `DotNetNuke.PowerBI\PBIEmbedded.Web\src\components\capacitySettings\capacitySettings.less`
6. `DotNetNuke.PowerBI\PBIEmbedded.Web\src\constants\actionTypes\capacitySettings.js`
7. `DotNetNuke.PowerBI\PBIEmbedded.Web\src\reducers\capacitySettings.js`

## Phase 2: Manual File Updates Required

### 1. Update `src\components\App.jsx`

**Replace the file contents with the version in `App_new.jsx` or make these changes:**

```jsx
// Add import
import CapacitySettings from "./capacitySettings";

// Update tab headers array (line ~35)
tabHeaders={[
    resx.get("GeneralSettings"),
    resx.get("Permissions"),
    resx.get("CapacitySettings"),      // ADD THIS LINE
    resx.get("CapacityManagement")
]}>

// Update tab components (line ~36)
<GeneralSettings />
<Permissions />
<CapacitySettings />                   // ADD THIS LINE
<CapacityManagement />
```

### 2. Update `src\constants\actionTypes\index.js`

**Add the import and merge:**

```javascript
import { capacitySettings } from "./capacitySettings";

const ActionTypes = {
    // ...existing action types...
    ...capacitySettings  // ADD THIS LINE
};

export default ActionTypes;
```

### 3. Update `src\reducers\index.js`

**Add import and include in combineReducers:**

```javascript
import capacitySettings from "./capacitySettings";

const rootReducer = combineReducers({
    settings,
    permissions,
    capacityManagement,
    capacitySettings  // ADD THIS LINE
});

export default rootReducer;
```

### 4. Update `src\actions\settings.js`

**Add these new action creators at the end of the file:**

```javascript
// Capacity Settings Actions
getCapacities(callback) {
    return (dispatch) => {
        dispatch({ type: ActionTypes.GET_CAPACITIES_STARTED });
        ApplicationService.getCapacities(data => {
            dispatch({
                type: ActionTypes.GET_CAPACITIES_SUCCESS,
                data: data.capacities || []
            });
            if (callback) callback(data);
        }, error => {
            dispatch({
                type: ActionTypes.GET_CAPACITIES_FAILED,
                data: error
            });
        });
    };
},

createCapacity(capacity, callback, failureCallback) {
    return (dispatch) => {
        dispatch({ type: ActionTypes.CREATE_CAPACITY_STARTED });
        ApplicationService.createCapacity(capacity, data => {
            dispatch({ type: ActionTypes.CREATE_CAPACITY_SUCCESS });
            if (callback) callback(data);
        }, error => {
            dispatch({ type: ActionTypes.CREATE_CAPACITY_FAILED, data: error });
            if (failureCallback) failureCallback(error);
        });
    };
},

updateCapacity(capacity, callback, failureCallback) {
    return (dispatch) => {
        dispatch({ type: ActionTypes.UPDATE_CAPACITY_STARTED });
        ApplicationService.updateCapacity(capacity, data => {
            dispatch({ type: ActionTypes.UPDATE_CAPACITY_SUCCESS });
            if (callback) callback(data);
        }, error => {
            dispatch({ type: ActionTypes.UPDATE_CAPACITY_FAILED, data: error });
            if (failureCallback) failureCallback(error);
        });
    };
},

deleteCapacity(payload, callback, failureCallback) {
    return (dispatch) => {
        dispatch({ type: ActionTypes.DELETE_CAPACITY_STARTED });
        ApplicationService.deleteCapacity(payload, data => {
            dispatch({ type: ActionTypes.DELETE_CAPACITY_SUCCESS });
            if (callback) callback(data);
        }, error => {
            dispatch({ type: ActionTypes.DELETE_CAPACITY_FAILED, data: error });
            if (failureCallback) failureCallback(error);
        });
    };
}
```

**Update existing capacity management actions to use CapacityId instead of SettingsId:**

Find and replace in `getCapacityStatus`, `startCapacity`, `pauseCapacity`, `getCapacityRules`:
- Change `SettingsId` to `CapacityId` in all method parameters and calls

Example:
```javascript
// BEFORE:
getCapacityRules(payload, callback, failureCallback) {
    return (dispatch) => {
        const settingsId = payload.SettingsId || payload;
        ApplicationService.getCapacityRules({ SettingsId: settingsId }, ...);
    };
}

// AFTER:
getCapacityRules(payload, callback, failureCallback) {
    return (dispatch) => {
        const capacityId = payload.CapacityId || payload;
        ApplicationService.getCapacityRules({ CapacityId: capacityId }, ...);
    };
}
```

### 5. Update `src\services\applicationService.js`

**Add new capacity settings methods:**

```javascript
// Add after deleteWorkspace method:

getCapacities(callback, failureCallback) {
    const sf = this.getServiceFramework("CapacitySettings");
    sf.get("GetCapacities", {}, callback, failureCallback);
}

createCapacity(payload, callback, failureCallback) {
    const sf = this.getServiceFramework("CapacitySettings");
    sf.post("CreateCapacity", payload, callback, failureCallback);
}

updateCapacity(payload, callback, failureCallback) {
    const sf = this.getServiceFramework("CapacitySettings");
    sf.post("UpdateCapacity", payload, callback, failureCallback);
}

deleteCapacity(payload, callback, failureCallback) {
    const sf = this.getServiceFramework("CapacitySettings");
    const capacityId = payload.CapacityId || payload;
    sf.post(`DeleteCapacity?capacityId=${capacityId}`, {}, callback, failureCallback);
}
```

**Update capacity management methods to use CapacityId:**

```javascript
// Update these methods:
getCapacityStatus(payload, callback, failureCallback) {
    const sf = this.getServiceFramework("CapacitySettings");  // Changed controller
    const capacityId = payload.CapacityId || payload;  // Changed parameter
    sf.get("GetCapacityStatus", { capacityId: capacityId }, callback, failureCallback);
}

startCapacity(payload, callback, failureCallback) {
    const sf = this.getServiceFramework("CapacitySettings");  // Changed controller
    const capacityId = payload.CapacityId || payload;  // Changed parameter
    const url = `StartCapacity?capacityId=${capacityId}`;  // Changed parameter name
    sf.post(url, {}, callback, failureCallback);
}

pauseCapacity(payload, callback, failureCallback) {
    const sf = this.getServiceFramework("CapacitySettings");  // Changed controller
    const capacityId = payload.CapacityId || payload;  // Changed parameter
    const url = `PauseCapacity?capacityId=${capacityId}`;  // Changed parameter name
    sf.post(url, {}, callback, failureCallback);
}

getCapacityRules(payload, callback, failureCallback) {
    const sf = this.getServiceFramework("CapacityManagement");
    const capacityId = payload.CapacityId || payload;  // Changed parameter
    sf.get("GetCapacityRules", { capacityId: capacityId }, callback, failureCallback);
}

createCapacityRule(rule, callback, failureCallback) {
    const sf = this.getServiceFramework("CapacityManagement");
    sf.post("CreateCapacityRule", rule, callback, failureCallback);  // Send full rule object
}

updateCapacityRule(rule, callback, failureCallback) {
    const sf = this.getServiceFramework("CapacityManagement");
    sf.post("UpdateCapacityRule", rule, callback, failureCallback);
}
```

### 6. Update `src\components\capacityManagement\capacityManagement.jsx`

This file needs significant updates. Here's a complete replacement section:

```jsx
import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { GridCell, GridSystem, DropdownWithError, InputGroup } from "@dnnsoftware/dnn-react-common";
import SettingsActions from "../../actions/settings";
import resx from "../../resources";
import CapacityRules from "./rule";
import CapacityStatus from "./capacityStatus";
import "./capacityManagement.less";

class CapacityManagement extends Component {
    constructor() {
        super();
        this.state = {
            selectedCapacity: "",
            error: {
                selectedCapacity: false
            }
        };
    }

    componentDidMount() {
        // Load capacities instead of workspaces
        this.props.dispatch(SettingsActions.getCapacities());
    }

    componentDidUpdate(prevProps) {
        // Auto-select first capacity when loaded
        if (!prevProps.capacities && this.props.capacities && this.props.capacities.length > 0 && !this.state.selectedCapacity) {
            const firstCapacity = this.props.capacities[0];
            this.setState({ selectedCapacity: firstCapacity.CapacityId });
            this.onCapacityChange(firstCapacity.CapacityId);
        }
    }

    onCapacityChange(capacityId) {
        this.props.dispatch(SettingsActions.clearCapacityData());

        this.setState({
            selectedCapacity: capacityId,
            error: {
                selectedCapacity: !capacityId
            }
        });

        if (capacityId) {
            // Load capacity rules
            this.props.dispatch(SettingsActions.getCapacityRules({
                CapacityId: parseInt(capacityId)
            }));
        }
    }

    getCapacityOptions() {
        let options = [];

        if (this.props.capacities && Array.isArray(this.props.capacities)) {
            options = this.props.capacities.map((item) => {
                return { 
                    label: item.CapacityDisplayName, 
                    value: item.CapacityId 
                };
            });
        }
        return options;
    }

    onRuleDeleted(ruleId) {
        const payload = {
            RuleId: ruleId
        };
        this.props.dispatch(SettingsActions.deleteCapacityRule(payload, () => {
            this.props.dispatch(SettingsActions.getCapacityRules({
                CapacityId: parseInt(this.state.selectedCapacity)
            }));
        }));
    }

    onRuleSaved(rule, editingRule) {
        const ruleData = {
            RuleName: rule.ruleName,
            RuleDescription: rule.ruleDescription,
            IsEnabled: rule.isEnabled,
            Action: rule.action,
            ExecutionTime: rule.executionTime,
            DaysOfWeek: rule.daysOfWeek,
            TimeZoneId: rule.timeZoneId,
            CapacityId: this.state.selectedCapacity  // Changed from SettingsId
        };
        
        if (editingRule) {
            ruleData.RuleId = editingRule.RuleId;
            this.props.dispatch(SettingsActions.updateCapacityRule(ruleData, () => {
                this.props.dispatch(SettingsActions.getCapacityRules({
                    CapacityId: parseInt(this.state.selectedCapacity)
                }));
            }));
        } else {
            this.props.dispatch(SettingsActions.createCapacityRule(ruleData, () => {
                this.props.dispatch(SettingsActions.getCapacityRules({
                    CapacityId: parseInt(this.state.selectedCapacity)
                }));
            }));
        }
    }

    render() {
        const { capacityRules, operationError } = this.props;
        const capacityOptions = this.getCapacityOptions();

        return (
            <div className="capacity-management-wrapper">
                <h2 className="capacity-management-title">{resx.get("CapacityManagement_Title")}</h2>
                <p className="capacity-management-description">{resx.get("CapacityManagement_Description")}</p>

                <div className="capacity-management-container">
                    <GridCell columnSize={100}>
                        <InputGroup>
                            <DropdownWithError
                                withLabel={true}
                                label={resx.get("SelectCapacity")}
                                tooltipMessage={resx.get("SelectCapacity_Help")}
                                error={this.state.error.selectedCapacity}
                                errorMessage={resx.get("SelectCapacity_Error")}
                                options={capacityOptions}
                                value={this.state.selectedCapacity}
                                onSelect={(option) => this.onCapacityChange(option.value)}
                                enabled={capacityOptions.length > 0}
                            />
                        </InputGroup>
                    </GridCell>

                    {this.state.selectedCapacity && (
                        <>
                            <CapacityStatus 
                                selectedCapacity={this.state.selectedCapacity}
                            />

                            <GridCell columnSize={100}>
                                {operationError && (
                                    <div className="capacity-operation-error">
                                        <div className="error-message">
                                            <button 
                                                className="error-close-btn"
                                                onClick={() => this.props.dispatch(SettingsActions.clearOperationError())}
                                                aria-label="Close error message"
                                            >
                                                ×
                                            </button>
                                            <strong>{resx.get("Capacity_Operation_Error_Title")}</strong>
                                            <p>{resx.get("Capacity_Operation_Error_Message")}</p>
                                            {operationError.message && (
                                                <small>{resx.get("Capacity_Operation_Error_Details")}: {operationError.message}</small>
                                            )}
                                        </div>
                                    </div>
                                )}
                            </GridCell>
                            <GridCell columnSize={100}>
                                <CapacityRules
                                    capacityRules={capacityRules || []}
                                    onDeleteRule={(ruleId) => this.onRuleDeleted(ruleId)}
                                    onSaveRule={(rule, editingRule) => this.onRuleSaved(rule, editingRule)}
                                />
                            </GridCell>
                        </>
                    )}
                </div>
            </div>
        );
    }
}

CapacityManagement.propTypes = {
    dispatch: PropTypes.func.isRequired,
    capacities: PropTypes.array,
    capacityStatus: PropTypes.object,
    capacityRules: PropTypes.array,
    capacityLoading: PropTypes.bool,
    capacityError: PropTypes.object,
    statusError: PropTypes.object,
    operationError: PropTypes.object
};

function mapStateToProps(state) {
    return {
        capacities: state.capacitySettings.capacities || [],  // Changed from workspaces
        capacityStatus: state.capacityManagement.capacityStatus,
        capacityRules: state.capacityManagement.capacityRules || [],
        capacityLoading: state.capacityManagement.loading,
        capacityError: state.capacityManagement.error,
        statusError: state.capacityManagement.statusError,
        operationError: state.capacityManagement.operationError
    };
}

export default connect(mapStateToProps)(CapacityManagement);
```

### 7. Update `src\components\capacityManagement\capacityStatus.jsx`

Update the component to use `selectedCapacity` prop and `CapacityId`:

```jsx
// In componentDidMount and polling:
this.props.dispatch(SettingsActions.getCapacityStatus({ 
    CapacityId: this.props.selectedCapacity  // Changed from SettingsId
}));

// In onStartCapacity:
this.props.dispatch(SettingsActions.startCapacity({ 
    CapacityId: this.props.selectedCapacity 
}, ...));

// In onPauseCapacity:
this.props.dispatch(SettingsActions.pauseCapacity({ 
    CapacityId: this.props.selectedCapacity 
}, ...));
```

### 8. Add Resource Strings

Add to `src\resources\[language].resx` or equivalent:

```xml
<data name="CapacitySettings" xml:space="preserve">
    <value>Capacity Settings</value>
</data>
<data name="CapacityManagement_Title" xml:space="preserve">
    <value>Manage Capacity Rules</value>
</data>
<data name="CapacityManagement_Description" xml:space="preserve">
    <value>Configure automated start/stop rules for your Power BI capacities</value>
</data>
<data name="SelectCapacity" xml:space="preserve">
    <value>Select Capacity</value>
</data>
<data name="SelectCapacity_Help" xml:space="preserve">
    <value>Choose the capacity you want to manage</value>
</data>
<data name="SelectCapacity_Error" xml:space="preserve">
    <value>Please select a capacity</value>
</data>
<!-- Add all other resource strings from REACT_UI_CHANGES_SUMMARY.md -->
```

## Testing Checklist

After making all changes:

1. **Build the React app:**
   ```bash
   cd C:\Dev\dnn.powerbiembedded\src\DotNetNuke.PowerBI\PBIEmbedded.Web
   npm install
   npm run build
   ```

2. **Verify tabs load:**
   - General Settings
   - Permissions
   - **Capacity Settings** (NEW)
   - Capacity Management

3. **Test Capacity Settings:**
   - Create a new capacity
   - Edit an existing capacity
   - Delete a capacity
   - Verify validation

4. **Test Capacity Management:**
   - Select a capacity from dropdown
   - View capacity status
   - Start/pause capacity
   - Create/edit/delete rules

## Common Issues and Solutions

### Issue: Capacity dropdown is empty
**Solution:** Check that `getCapacities()` action is dispatching correctly and the API endpoint is working.

### Issue: Rules not saving with CapacityId
**Solution:** Verify `onRuleSaved` method is using `CapacityId` instead of `SettingsId`.

### Issue: Resource strings not showing
**Solution:** Add all resource keys to the appropriate `.resx` files and rebuild.

### Issue: Reducer not updating
**Solution:** Ensure `capacitySettings` reducer is properly imported and added to `combineReducers`.

## Files Modified Summary

- ? Created: 7 new files for capacity settings
- ?? Update: `App.jsx`
- ?? Update: `actionTypes/index.js`
- ?? Update: `reducers/index.js`
- ?? Update: `actions/settings.js`
- ?? Update: `services/applicationService.js`
- ?? Update: `capacityManagement/capacityManagement.jsx`
- ?? Update: `capacityManagement/capacityStatus.jsx`
- ?? Update: Resource files
