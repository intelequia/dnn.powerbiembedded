# Capacity Settings Decoupling - Implementation Summary

## Overview
This implementation decouples Power BI Embedded capacity management settings from workspace settings by creating a new `PBI_CapacitySettings` table. This allows registering and managing capacities independently, with capacity rules tied directly to capacities rather than individual workspaces.

## Key Architecture Decisions

1. **No Workspace-Capacity Linking**: Workspaces do NOT reference capacities. Capacity management is completely independent from workspace configuration.
2. **Rules Tied to Capacities**: Capacity rules reference capacities directly via `CapacityId`
3. **No Backward Compatibility**: This is a new feature with no migration from old settings needed

## Changes Made

### 1. Database Changes

#### New Table: `PBI_CapacitySettings`
Created a new table to store capacity configurations independently.

**SQL Script:** `01.03.01.SqlDataProvider`

**Key Fields:**
- `CapacityId` (Primary Key)
- `CapacityName` - Unique identifier for the capacity
- `CapacityDisplayName` - Friendly name for display
- `Description` - Optional description
- Service Principal authentication settings (only authentication method supported):
  - `ServicePrincipalApplicationId`
  - `ServicePrincipalApplicationSecret`
  - `ServicePrincipalTenant`
- Azure Management API settings:
  - `AzureManagementSubscriptionId`
  - `AzureManagementResourceGroup`
  - `AzureManagementCapacityName`
  - `AzureManagementPollingInterval` (default: 60 minutes)
- `DisabledCapacityMessage` - Custom message when capacity is disabled
- `IsEnabled`, `IsDeleted` - Status flags
- Audit fields (CreatedOn, CreatedBy, ModifiedOn, ModifiedBy)

#### Updated Table: `PBI_CapacityRules`
- **Replaced** `SettingsId` with `CapacityId` to link rules to capacities
- Added foreign key constraint to reference `PBI_CapacitySettings`
- Rules are now managed per capacity, not per workspace

### 2. New Data Models

#### `CapacitySettings` Model (`Data\CapacitySettings\Models\CapacitySettings.cs`)
- Represents a Power BI Embedded capacity configuration
- Contains all Azure Management API credentials and settings
- Uses Service Principal authentication only (simplified from supporting multiple auth methods)
- Includes soft delete and audit trail support
- Default polling interval: 60 minutes

### 3. New Repositories

#### `ICapacitySettingsRepository` & `CapacitySettingsRepository`
Located in `Data\CapacitySettings\`

**Methods:**
- `GetCapacityById(int capacityId, int portalId)` - Retrieve by ID
- `GetCapacityByName(string capacityName, int portalId)` - Retrieve by name
- `GetCapacities(int portalId)` - Get all capacities for a portal
- `GetAllCapacities()` - Get all capacities across all portals
- `GetEnabledCapacities(int portalId)` - Get only enabled capacities
- `AddCapacity(CapacitySettings, int portalId, int? userId)` - Create new capacity
- `UpdateCapacity(CapacitySettings, int portalId)` - Update existing capacity
- `DeleteCapacity(int capacityId, int portalId)` - Soft delete capacity

### 4. Updated Repositories

#### `CapacityRulesRepository`
- **Replaced** `GetRulesBySettingsId` with `GetRulesByCapacityId`
- **Replaced** `DeleteRulesBySettingsId` with `DeleteRulesByCapacityId`
- Updated `AddRule` and `UpdateRule` to use `CapacityId` only

### 5. New Controllers

#### `CapacitySettingsController` (`Services\CapacitySettingsController.cs`)
New Web API controller for managing capacity settings.

**Endpoints:**
- `GET /GetCapacities` - List all capacities
- `GET /GetCapacity?capacityId={id}` - Get specific capacity
- `POST /CreateCapacity` - Create new capacity
- `POST /UpdateCapacity` - Update existing capacity
- `POST /DeleteCapacity?capacityId={id}` - Delete capacity
- `GET /GetCapacityStatus?capacityId={id}` - Get Azure status
- `POST /StartCapacity?capacityId={id}` - Start Azure capacity
- `POST /PauseCapacity?capacityId={id}` - Pause Azure capacity

### 6. Updated Controllers

#### `CapacityManagementController`
- Simplified to only manage capacity rules
- Removed capacity start/stop/status methods (moved to CapacitySettingsController)
- `GetCapacityRules` requires `capacityId` parameter
- All CRUD operations for rules

### 7. Updated Services

#### `ICapacityManagementService`
- Only accepts `CapacitySettings` parameter (no PowerBISettings support)
- Clean interface with no deprecated methods

#### `CapacityManagementService` 
- Simplified implementation working directly with `CapacitySettings`
- No wrapper pattern needed
- All methods accept only `CapacitySettings`

### 8. Updated Models

#### `PowerBISettings` (`SharedSettings.cs`)
- **Removed** all Azure Management fields:
  - `AzureManagementSubscriptionId`
  - `AzureManagementResourceGroup`
  - `AzureManagementCapacityName`
  - `AzureManagementPollingInterval`
  - `CapacityId`
- Workspaces are completely independent from capacity management

#### `CapacityRule`
- **Replaced** `SettingsId` with `CapacityId`
- Rules are now tied to capacities, not workspaces

### 9. Updated Tasks

#### `CapacityRuleTask`
- Refactored to work with capacities instead of workspace settings
- Iterates through `CapacitySettings` instead of `PowerBISettings`
- Uses `GetRulesByCapacityId` instead of `GetRulesBySettingsId`

## Usage Examples

### Registering a New Capacity

```csharp
var capacity = new CapacitySettings
{
    CapacityName = "prod-capacity-01",
    CapacityDisplayName = "Production Capacity 01",
    Description = "Primary production capacity for North America",
    ServicePrincipalApplicationId = "app-id",
    ServicePrincipalApplicationSecret = "secret",
    ServicePrincipalTenant = "tenant-id",
    AzureManagementSubscriptionId = "subscription-id",
    AzureManagementResourceGroup = "resource-group",
    AzureManagementCapacityName = "capacity-name",
    AzureManagementPollingInterval = 60, // Default is 60 minutes
    IsEnabled = true
};

CapacitySettingsRepository.Instance.AddCapacity(capacity, portalId, userId);
```

### Creating a Capacity Rule

```csharp
var rule = new CapacityRule
{
    CapacityId = capacityId,
    RuleName = "Weekday Morning Start",
    RuleDescription = "Start capacity at 8 AM on weekdays",
    RuleType = "TimeBasedStart",
    ExecutionTime = new TimeSpan(8, 0, 0),
    DaysOfWeek = "1,2,3,4,5", // Monday-Friday
    Action = "Start",
    TimeZoneId = "Eastern Standard Time",
    IsEnabled = true
};

CapacityRulesRepository.Instance.AddRule(rule);
```

### Managing Capacity via API

```javascript
// Get all capacities
GET /API/CapacitySettings/GetCapacities

// Create new capacity
POST /API/CapacitySettings/CreateCapacity
{
    "CapacityName": "prod-capacity-01",
    "CapacityDisplayName": "Production Capacity 01",
    "Description": "Primary production capacity",
    "ServicePrincipalApplicationId": "app-id",
    "ServicePrincipalApplicationSecret": "secret",
    "ServicePrincipalTenant": "tenant-id",
    "AzureManagementSubscriptionId": "subscription-id",
    "AzureManagementResourceGroup": "resource-group",
    "AzureManagementCapacityName": "capacity-name",
    "AzureManagementPollingInterval": 60,
    "IsEnabled": true
}

// Start a capacity
POST /API/CapacitySettings/StartCapacity?capacityId=1

// Get capacity status
GET /API/CapacitySettings/GetCapacityStatus?capacityId=1

// Get rules for a capacity
GET /API/CapacityManagement/GetCapacityRules?capacityId=1
```

## Benefits

1. **Complete Separation**: Capacity management is fully independent of workspace configuration
2. **Reusability**: Rules can be easily managed per capacity
3. **Clarity**: Clear ownership - capacities manage Azure resources, workspaces manage Power BI content
4. **Maintainability**: Updating capacity credentials is centralized
5. **Scalability**: Easy to register and manage multiple capacities
6. **Flexibility**: Capacities can be managed without any workspace dependencies

## Files Created

- `DotNetNuke.PowerBI\Providers\DataProviders\SqlDataProvider\01.03.01.SqlDataProvider` (updated)
- `DotNetNuke.PowerBI\Data\CapacitySettings\Models\CapacitySettings.cs`
- `DotNetNuke.PowerBI\Data\CapacitySettings\ICapacitySettingsRepository.cs`
- `DotNetNuke.PowerBI\Data\CapacitySettings\CapacitySettingsRepository.cs`
- `DotNetNuke.PowerBI\Services\CapacitySettingsController.cs`
- `DotNetNuke.PowerBI\Services\CapacityManagementService.cs` (recreated)

## Files Modified

- `DotNetNuke.PowerBI\Data\SharedSettings\Models\SharedSettings.cs` - Removed Azure Management fields
- `DotNetNuke.PowerBI\Data\SharedSettings\SharedSettingsRepository.cs` - Removed Azure Management field updates
- `DotNetNuke.PowerBI\Data\CapacityRules\Models\CapacityRule.cs` - Replaced SettingsId with CapacityId
- `DotNetNuke.PowerBI\Data\CapacityRules\ICapacityRulesRepository.cs` - Updated method signatures
- `DotNetNuke.PowerBI\Data\CapacityRules\CapacityRulesRepository.cs` - Updated implementation
- `DotNetNuke.PowerBI\Services\ICapacityManagementService.cs` - Simplified to only use CapacitySettings
- `DotNetNuke.PowerBI\Services\CapacityManagementController.cs` - Simplified to only manage rules
- `DotNetNuke.PowerBI\Tasks\CapacityRuleTask.cs` - Refactored to use CapacitySettings
