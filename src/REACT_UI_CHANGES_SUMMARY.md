# React Application Changes for Capacity Settings Decoupling

## Overview
This document outlines all the changes needed to update the React application to work with the new decoupled capacity settings structure.

## Files Created

### 1. Capacity Settings Components

#### `DotNetNuke.PowerBI\PBIEmbedded.Web\src\components\capacitySettings\`
- ? `index.jsx` - Entry point for capacity settings component
- ? `capacitySettings.jsx` - Main component for managing capacities list
- ? `capacityEditor.jsx` - Modal form for creating/editing capacities
- ? `capacityRow.jsx` - Row component for displaying capacity in list
- ? `capacitySettings.less` - Styles for capacity settings

## Files to Update

### 2. Main App Component

#### `DotNetNuke.PowerBI\PBIEmbedded.Web\src\components\App.jsx`
**Change:** Add new "Capacity Settings" tab between "Permissions" and "Capacity Management"

```jsx
import CapacitySettings from "./capacitySettings";

// In render():
tabHeaders={[
    resx.get("GeneralSettings"),
    resx.get("Permissions"),
    resx.get("CapacitySettings"),  // NEW TAB
    resx.get("CapacityManagement")
]}>
    <GeneralSettings />
    <Permissions />
    <CapacitySettings />  // NEW COMPONENT
    <CapacityManagement />
```

### 3. Capacity Management Component

#### `DotNetNuke.PowerBI\PBIEmbedded.Web\src\components\capacityManagement\capacityManagement.jsx`

**Changes needed:**
1. Replace workspace dropdown with capacity dropdown
2. Update state to use `selectedCapacity` instead of `selectedWorkspace`
3. Update API calls to use `CapacityId` instead of `SettingsId`
4. Remove workspace configuration validation
5. Update props and mapStateToProps

**Key Changes:**
```jsx
// OLD:
this.setState({ selectedWorkspace: workspaceId });
this.props.dispatch(SettingsActions.getCapacityRules({ SettingsId: workspaceId }));

// NEW:
this.setState({ selectedCapacity: capacityId });
this.props.dispatch(SettingsActions.getCapacityRules({ CapacityId: capacityId }));
```

### 4. Application Service

#### `DotNetNuke.PowerBI\PBIEmbedded.Web\src\services\applicationService.js`

**Add new methods:**
```javascript
// Capacity Settings API
getCapacities(callback) {
    const sf = this.getServiceFramework("CapacitySettings");
    sf.get("GetCapacities", {}, callback);
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
    sf.post("DeleteCapacity", payload, callback, failureCallback);
}
```

**Update existing methods to use CapacityId:**
```javascript
// OLD:
getCapacityStatus(payload, callback, failureCallback) {
    const settingsId = payload.SettingsId || payload;
    sf.get("GetCapacityStatus", { SettingsId: settingsId }, ...);
}

// NEW:
getCapacityStatus(payload, callback, failureCallback) {
    const capacityId = payload.CapacityId || payload;
    sf.get("GetCapacityStatus", { capacityId: capacityId }, ...);
}
```

### 5. Actions

#### `DotNetNuke.PowerBI\PBIEmbedded.Web\src\actions\settings.js`

**Add new actions:**
```javascript
getCapacities(callback) {
    return (dispatch) => {
        dispatch({ type: CapacitySettingsActionTypes.GET_CAPACITIES_STARTED });
        ApplicationService.getCapacities(data => {
            dispatch({
                type: CapacitySettingsActionTypes.GET_CAPACITIES_SUCCESS,
                data: data.capacities
            });
            if (callback) callback(data);
        });
    };
}

createCapacity(capacity, callback) {
    return (dispatch) => {
        ApplicationService.createCapacity(capacity, data => {
            dispatch({ type: CapacitySettingsActionTypes.CREATE_CAPACITY_SUCCESS });
            if (callback) callback(data);
        });
    };
}

updateCapacity(capacity, callback) {
    return (dispatch) => {
        ApplicationService.updateCapacity(capacity, data => {
            dispatch({ type: CapacitySettingsActionTypes.UPDATE_CAPACITY_SUCCESS });
            if (callback) callback(data);
        });
    };
}

deleteCapacity(payload, callback) {
    return (dispatch) => {
        ApplicationService.deleteCapacity(payload, data => {
            dispatch({ type: CapacitySettingsActionTypes.DELETE_CAPACITY_SUCCESS });
            if (callback) callback(data);
        });
    };
}
```

**Update capacity management actions to use CapacityId:**
```javascript
// Change all instances of SettingsId to CapacityId
getCapacityStatus(payload, callback, failureCallback) {
    return (dispatch) => {
        dispatch({ type: CapacityActionTypes.GET_CAPACITY_STATUS_STARTED });
        ApplicationService.getCapacityStatus(payload, data => {
            dispatch({
                type: CapacityActionTypes.GET_CAPACITY_STATUS_SUCCESS,
                data: data
            });
            if (callback) callback(data);
        }, failureCallback);
    };
}
```

### 6. Action Types

#### `DotNetNuke.PowerBI\PBIEmbedded.Web\src\constants\actionTypes\`

**Add new file: `capacitySettings.js`**
```javascript
export const capacitySettings = {
    _STARTED: "GET_CAPACITIES_STARTED",
    GET_CAPACITIES_SUCCESS: "GET_CAPACITIES_SUCCESS",
    GET_CAPACITIES_FAILED: "GET_CAPACITIES_FAILED",
    CREATE_CAPACITY_SUCCESS: "CREATE_CAPACITY_SUCCESS",
    UPDATE_CAPACITY_SUCCESS: "UPDATE_CAPACITY_SUCCESS",
    DELETE_CAPACITY_SUCCESS: "DELETE_CAPACITY_SUCCESS"
};
```

**Update `index.js`:**
```javascript
import { capacitySettings } from "./capacitySettings";

const ActionTypes = {
    // ...existing types
    ...capacitySettings
};

export default ActionTypes;
```

### 7. Reducers

#### `DotNetNuke.PowerBI\PBIEmbedded.Web\src\reducers\`

**Add new file: `capacitySettings.js`**
```javascript
import ActionTypes from "../constants/actionTypes";

export default function capacitySettingsReducer(state = {
    capacities: [],
    loading: false,
    error: null
}, action) {
    switch (action.type) {
        case ActionTypes.GET_CAPACITIES_STARTED:
            return { ...state, loading: true, error: null };
        case ActionTypes.GET_CAPACITIES_SUCCESS:
            return { ...state, capacities: action.data, loading: false };
        case ActionTypes.GET_CAPACITIES_FAILED:
            return { ...state, error: action.data, loading: false };
        default:
            return state;
    }
}
```

**Update `index.js`:**
```javascript
import capacitySettings from "./capacitySettings";

const rootReducer = combineReducers({
    settings,
    permissions,
    capacityManagement,
    capacitySettings  // ADD THIS
});
```

### 8. Resources (Localization)

#### `DotNetNuke.PowerBI\PBIEmbedded.Web\src\resources\`

**Add new resource keys:**
```javascript
{
    // Capacity Settings Tab
    "CapacitySettings": "Capacity Settings",
    "CapacitySettings_Title": "Manage Power BI Capacities",
    "CapacitySettings_Description": "Configure and manage Azure Power BI Embedded capacities for scheduling and automation",
    "CapacitySettings_BasicInfo": "Basic Information",
    "CapacitySettings_Authentication": "Service Principal Authentication",
    "CapacitySettings_AzureManagement": "Azure Management",
    "CapacitySettings_Advanced": "Advanced Settings",
    
    // Capacity Form Fields
    "CapacityName": "Capacity Name",
    "CapacityName_Error": "Capacity name is required",
    "CapacityName_Help": "Unique identifier for this capacity configuration",
    "CapacityDisplayName": "Display Name",
    "CapacityDisplayName_Error": "Display name is required",
    "CapacityDisplayName_Help": "Friendly name shown in the UI",
    "Description": "Description",
    "Description_Help": "Optional description for this capacity",
    "ServicePrincipalApplicationId": "Application ID",
    "ServicePrincipalApplicationId_Error": "Application ID is required",
    "ServicePrincipalApplicationId_Help": "Azure AD Service Principal Application (Client) ID",
    "ServicePrincipalApplicationSecret": "Application Secret",
    "ServicePrincipalApplicationSecret_Error": "Application Secret is required",
    "ServicePrincipalApplicationSecret_Help": "Azure AD Service Principal Application Secret",
    "ServicePrincipalTenant": "Tenant ID",
    "ServicePrincipalTenant_Error": "Tenant ID is required",
    "ServicePrincipalTenant_Help": "Azure AD Tenant ID",
    "AzureManagementSubscriptionId": "Subscription ID",
    "AzureManagementSubscriptionId_Error": "Subscription ID is required",
    "AzureManagementSubscriptionId_Help": "Azure subscription ID where the capacity is located",
    "AzureManagementResourceGroup": "Resource Group",
    "AzureManagementResourceGroup_Error": "Resource Group is required",
    "AzureManagementResourceGroup_Help": "Azure resource group name",
    "AzureManagementCapacityName": "Capacity Name",
    "AzureManagementCapacityName_Error": "Capacity name is required",
    "AzureManagementCapacityName_Help": "Name of the Power BI Embedded capacity in Azure",
    "AzureManagementPollingInterval": "Polling Interval (minutes)",
    "AzureManagementPollingInterval_Error": "Polling interval must be at least 1 minute",
    "AzureManagementPollingInterval_Help": "How often to check capacity status (in minutes)",
    "DisabledCapacityMessage": "Disabled Message",
    "DisabledCapacityMessage_Help": "Custom message to display when capacity is disabled",
    "IsEnabled": "Enabled",
    
    // Actions
    "AddCapacity": "Add Capacity",
    "EditCapacity": "Edit Capacity",
    "NoCapacitiesConfigured": "No capacities have been configured yet. Click 'Add Capacity' to create one.",
    "ConfirmDeleteCapacity": "Are you sure you want to delete the capacity '{0}'?",
    "Enabled": "Enabled",
    "Disabled": "Disabled",
    "NoDescription": "No description",
    "Minutes": "minutes"
}
```

## Implementation Checklist

- [?] Created Capacity Settings components
- [ ] Update App.jsx to include new tab
- [ ] Update capacityManagement.jsx to use capacities instead of workspaces
- [ ] Update applicationService.js with new API methods
- [ ] Add capacity settings actions
- [ ] Add capacity settings action types
- [ ] Add capacity settings reducer
- [ ] Add resource strings
- [ ] Test complete flow

## API Endpoints Summary

### New Endpoints (Capacity Settings)
- GET `/API/CapacitySettings/GetCapacities`
- POST `/API/CapacitySettings/CreateCapacity`
- POST `/API/CapacitySettings/UpdateCapacity`
- POST `/API/CapacitySettings/DeleteCapacity?capacityId={id}`

### Updated Endpoints (Capacity Management)
- GET `/API/CapacityManagement/GetCapacityStatus?capacityId={id}` (was settingsId)
- POST `/API/CapacityManagement/StartCapacity?capacityId={id}` (was settingsId)
- POST `/API/CapacityManagement/PauseCapacity?capacityId={id}` (was settingsId)
- GET `/API/CapacityManagement/GetCapacityRules?capacityId={id}` (was settingsId)
- POST `/API/CapacityManagement/CreateCapacityRule` (use capacityId in body)
- POST `/API/CapacityManagement/UpdateCapacityRule` (use capacityId in body)
- POST `/API/CapacityManagement/DeleteCapacityRule?ruleId={id}` (unchanged)

## Testing Checklist

1. **Capacity Settings Tab**
   - [ ] Can create new capacity
   - [ ] Can edit existing capacity
   - [ ] Can delete capacity
   - [ ] Form validation works
   - [ ] Service Principal fields are required
   - [ ] Polling interval validates correctly

2. **Capacity Management Tab**
   - [ ] Capacity dropdown shows all capacities
   - [ ] Can select a capacity
   - [ ] Can view capacity status
   - [ ] Can start/pause capacity
   - [ ] Can create/edit/delete rules for capacity
   - [ ] Rules are properly linked to capacity

3. **Integration**
   - [ ] All tabs load without errors
   - [ ] State is properly managed
   - [ ] API calls use correct endpoints
   - [ ] Error handling works
   - [ ] Loading states display correctly
