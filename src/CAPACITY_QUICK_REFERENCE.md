# Quick Reference Guide - Capacity Settings

## For Developers

### Registering a New Capacity

```csharp
// Create capacity settings
var capacity = new CapacitySettings
{
    CapacityName = "my-capacity",
    CapacityDisplayName = "My Production Capacity",
    Description = "Primary capacity for production workloads",
    ServicePrincipalApplicationId = "YOUR_APP_ID",
    ServicePrincipalApplicationSecret = "YOUR_APP_SECRET",
    ServicePrincipalTenant = "YOUR_TENANT_ID",
    AzureManagementSubscriptionId = "YOUR_SUBSCRIPTION_ID",
    AzureManagementResourceGroup = "YOUR_RESOURCE_GROUP",
    AzureManagementCapacityName = "YOUR_CAPACITY_NAME",
    AzureManagementPollingInterval = 60,
    IsEnabled = true
};

// Save to database
CapacitySettingsRepository.Instance.AddCapacity(capacity, portalId, userId);
```

### Creating Capacity Rules

```csharp
// Start rule - weekdays at 8 AM
var startRule = new CapacityRule
{
    CapacityId = capacityId,
    RuleName = "Morning Start",
    RuleDescription = "Start capacity at 8 AM on weekdays",
    RuleType = "TimeBasedStart",
    ExecutionTime = new TimeSpan(8, 0, 0),
    DaysOfWeek = "1,2,3,4,5", // Monday-Friday
    Action = "Start",
    TimeZoneId = "Eastern Standard Time",
    IsEnabled = true
};

CapacityRulesRepository.Instance.AddRule(startRule);

// Stop rule - weekdays at 6 PM
var stopRule = new CapacityRule
{
    CapacityId = capacityId,
    RuleName = "Evening Stop",
    RuleDescription = "Pause capacity at 6 PM on weekdays",
    RuleType = "TimeBasedStop",
    ExecutionTime = new TimeSpan(18, 0, 0),
    DaysOfWeek = "1,2,3,4,5",
    Action = "Stop",
    TimeZoneId = "Eastern Standard Time",
    IsEnabled = true
};

CapacityRulesRepository.Instance.AddRule(stopRule);
```

### Managing Capacity Programmatically

```csharp
// Get capacity settings
var capacity = CapacitySettingsRepository.Instance.GetCapacityById(capacityId, portalId);

// Create service instance
var capacityService = new CapacityManagementService();

// Get current status
var status = await capacityService.GetCapacityStatusAsync(capacity);
Console.WriteLine($"Capacity: {status.DisplayName}, State: {status.State}");

// Start capacity
if (status.State != "active")
{
    bool started = await capacityService.StartCapacityAsync(capacity);
    if (started)
    {
        Console.WriteLine("Capacity started successfully");
    }
}

// Pause capacity
bool paused = await capacityService.PauseCapacityAsync(capacity);
if (paused)
{
    Console.WriteLine("Capacity paused successfully");
}

// Check if running
bool isRunning = await capacityService.IsCapacityRunningAsync(capacity);
```

## For API Consumers

### REST API Endpoints

#### Capacity Management

```http
# Get all capacities for the current portal
GET /API/CapacitySettings/GetCapacities

# Get specific capacity
GET /API/CapacitySettings/GetCapacity?capacityId=1

# Create new capacity
POST /API/CapacitySettings/CreateCapacity
Content-Type: application/json

{
  "CapacityName": "my-capacity",
  "CapacityDisplayName": "My Production Capacity",
  "Description": "Primary capacity",
  "ServicePrincipalApplicationId": "app-id",
  "ServicePrincipalApplicationSecret": "secret",
  "ServicePrincipalTenant": "tenant-id",
  "AzureManagementSubscriptionId": "subscription-id",
  "AzureManagementResourceGroup": "resource-group",
  "AzureManagementCapacityName": "capacity-name",
  "AzureManagementPollingInterval": 60,
  "IsEnabled": true
}

# Update capacity
POST /API/CapacitySettings/UpdateCapacity
Content-Type: application/json

{
  "CapacityId": 1,
  "CapacityName": "my-capacity",
  // ... other fields
}

# Delete capacity
POST /API/CapacitySettings/DeleteCapacity?capacityId=1
```

#### Capacity Operations

```http
# Get Azure capacity status
GET /API/CapacitySettings/GetCapacityStatus?capacityId=1

Response:
{
  "DisplayName": "my-capacity",
  "State": "active",
  "Region": "East US",
  "Sku": "A1"
}

# Start capacity
POST /API/CapacitySettings/StartCapacity?capacityId=1

Response:
{
  "message": "Capacity started successfully"
}

# Pause capacity
POST /API/CapacitySettings/PauseCapacity?capacityId=1

Response:
{
  "message": "Capacity paused successfully"
}
```

#### Capacity Rules

```http
# Get all rules for a capacity
GET /API/CapacityManagement/GetCapacityRules?capacityId=1

# Create capacity rule
POST /API/CapacityManagement/CreateCapacityRule
Content-Type: application/json

{
  "CapacityId": 1,
  "RuleName": "Morning Start",
  "RuleDescription": "Start capacity at 8 AM on weekdays",
  "RuleType": "TimeBasedStart",
  "ExecutionTime": "08:00:00",
  "DaysOfWeek": "1,2,3,4,5",
  "Action": "Start",
  "TimeZoneId": "Eastern Standard Time",
  "IsEnabled": true
}

# Update capacity rule
POST /API/CapacityManagement/UpdateCapacityRule
Content-Type: application/json

{
  "RuleId": 1,
  "CapacityId": 1,
  "RuleName": "Updated Rule Name",
  // ... other fields
}

# Delete capacity rule
POST /API/CapacityManagement/DeleteCapacityRule?ruleId=1
```

## Database Schema Reference

### PBI_CapacitySettings Table

| Column | Type | Description |
|--------|------|-------------|
| CapacityId | int | Primary key, auto-increment |
| PortalId | int | Portal ID |
| CapacityName | nvarchar(100) | Unique capacity identifier |
| CapacityDisplayName | nvarchar(200) | Display name |
| Description | nvarchar(500) | Optional description |
| ServicePrincipalApplicationId | nvarchar(250) | Service Principal App ID (required) |
| ServicePrincipalApplicationSecret | nvarchar(250) | Service Principal Secret (required) |
| ServicePrincipalTenant | nvarchar(250) | Service Principal Tenant ID (required) |
| AzureManagementSubscriptionId | nvarchar(100) | Azure subscription ID |
| AzureManagementResourceGroup | nvarchar(100) | Azure resource group name |
| AzureManagementCapacityName | nvarchar(100) | Azure capacity name |
| AzureManagementPollingInterval | int | Polling interval in minutes (default: 60) |
| DisabledCapacityMessage | nvarchar(500) | Message when capacity is disabled |
| IsEnabled | bit | Whether capacity is enabled |
| CreatedOn | datetime | Creation timestamp |
| CreatedBy | int | User ID who created |
| ModifiedOn | datetime | Last modification timestamp |
| ModifiedBy | int | User ID who last modified |
| IsDeleted | bit | Soft delete flag |

### PBI_Settings Table (Updated)

| Column | Type | Description |
|--------|------|-------------|
| ... | ... | Existing columns |
| ~~CapacityId~~ | ... | Not used - capacities are independent |
| ~~AzureManagement*~~ | ... | Not used - moved to PBI_CapacitySettings |

### PBI_CapacityRules Table (Updated)

| Column | Type | Description |
|--------|------|-------------|
| RuleId | int | Primary key, auto-increment |
| PortalId | int | Portal ID |
| CapacityId | int | FK to PBI_CapacitySettings (required) |
| ... | ... | Other rule fields |

## Common Scenarios

### Scenario 1: Registering Multiple Capacities

```csharp
// Development capacity
var devCapacity = new CapacitySettings
{
    CapacityName = "dev-capacity",
    CapacityDisplayName = "Development Capacity",
    Description = "Development environment capacity",
    ServicePrincipalApplicationId = "dev-app-id",
    ServicePrincipalApplicationSecret = "dev-secret",
    ServicePrincipalTenant = "tenant-id",
    AzureManagementSubscriptionId = "subscription-id",
    AzureManagementResourceGroup = "dev-resource-group",
    AzureManagementCapacityName = "mycompany-dev",
    AzureManagementPollingInterval = 60,
    IsEnabled = true
};
CapacitySettingsRepository.Instance.AddCapacity(devCapacity, portalId, userId);

// Production capacity
var prodCapacity = new CapacitySettings
{
    CapacityName = "prod-capacity",
    CapacityDisplayName = "Production Capacity",
    Description = "Production environment capacity",
    ServicePrincipalApplicationId = "prod-app-id",
    ServicePrincipalApplicationSecret = "prod-secret",
    ServicePrincipalTenant = "tenant-id",
    AzureManagementSubscriptionId = "subscription-id",
    AzureManagementResourceGroup = "prod-resource-group",
    AzureManagementCapacityName = "mycompany-prod",
    AzureManagementPollingInterval = 60,
    IsEnabled = true
};
CapacitySettingsRepository.Instance.AddCapacity(prodCapacity, portalId, userId);
```

### Scenario 2: Cost Optimization with Schedules

```csharp
// Office hours only (8 AM - 6 PM, weekdays)
var capacity = CapacitySettingsRepository.Instance.GetCapacityByName("office-hours-capacity", portalId);

// Start rule
var startRule = new CapacityRule
{
    CapacityId = capacity.CapacityId,
    RuleName = "Office Hours Start",
    ExecutionTime = new TimeSpan(8, 0, 0),
    DaysOfWeek = "1,2,3,4,5",
    Action = "Start",
    IsEnabled = true
};
CapacityRulesRepository.Instance.AddRule(startRule);

// Stop rule
var stopRule = new CapacityRule
{
    CapacityId = capacity.CapacityId,
    RuleName = "Office Hours Stop",
    ExecutionTime = new TimeSpan(18, 0, 0),
    DaysOfWeek = "1,2,3,4,5",
    Action = "Stop",
    IsEnabled = true
};
CapacityRulesRepository.Instance.AddRule(stopRule);
```

## Troubleshooting

### Issue: Capacity not starting/stopping

1. Check Azure credentials in capacity settings
2. Verify service principal has proper permissions
3. Check Azure subscription, resource group, and capacity names are correct
4. Review DNN event logs for detailed error messages

### Issue: Workspaces not using capacity

1. Verify `CapacityId` is set in workspace settings
2. Check if capacity is enabled (`IsEnabled = true`)
3. Ensure capacity is not soft-deleted (`IsDeleted = false`)
4. Verify workspace's Power BI workspace is assigned to the Azure capacity

### Issue: Rules not executing

1. Check if capacity rule is enabled (`IsEnabled = true`)
2. Verify `CapacityId` is correctly set
3. Check DNN scheduled task "Capacity Rule Evaluator" is running
4. Review event logs for rule execution errors
5. Verify time zone settings match your expected execution time

## Best Practices

1. **Naming Convention:** Use descriptive names for capacities (e.g., "prod-eastus-a1", "dev-westus-a2")
2. **Credentials Management:** Use Service Principal authentication for production
3. **Rule Design:** Create separate start/stop rules rather than combined rules
4. **Testing:** Test capacity operations in development before deploying to production
5. **Monitoring:** Regularly review capacity status and rule execution logs
6. **Documentation:** Document which workspaces are linked to which capacities
7. **Security:** Rotate service principal secrets regularly
8. **Cost Control:** Use capacity rules to automatically pause capacities during off-hours
