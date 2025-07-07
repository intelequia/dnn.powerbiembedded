# Custom RLS Extension Examples

This document provides examples of how to implement custom RLS (Role Level Security) extensions for the DNN Power BI Embedded module.

## ⚠️ Important: DNN User Roles Are Always Passed Automatically

**Before implementing custom extensions**: The DNN Power BI Embedded module automatically passes all DNN user roles to Power BI with every request. You do **not** need to implement custom extensions just to pass user roles to Power BI.

Custom extensions are intended for:
- Passing user identifiers from external systems
- Creating composite identifiers combining multiple data sources
- Implementing complex business logic for user identification
- Integrating with external databases or APIs

**For simple role-based filtering, configure your Power BI dataset directly using the DNN roles that are automatically passed.**

## Interface Definition

All custom RLS extensions must implement the `IRlsCustomExtension` interface:

```csharp
namespace DotNetNuke.PowerBI.Extensibility
{
    public interface IRlsCustomExtension
    {
        string GetRlsValue(HttpContext httpContext);
    }
}
```

## Example Implementations

### 1. Basic Department-Based Extension

```csharp
using System;
using System.Web;
using DotNetNuke.Entities.Users;
using DotNetNuke.Common;
using DotNetNuke.PowerBI.Extensibility;

namespace MyCompany.PowerBI.Extensions
{
    public class DepartmentRlsExtension : IRlsCustomExtension
    {
        public string GetRlsValue(HttpContext httpContext)
        {
            try
            {
                var userInfo = UserController.Instance.GetCurrentUserInfo();
                
                // Get department from profile
                var department = userInfo.Profile.GetProperty("Department")?.PropertyValue;
                
                // Fallback to a default or throw exception
                if (string.IsNullOrEmpty(department))
                {
                    department = "General"; // or throw new Exception("Department not set for user");
                }
                
                return department;
            }
            catch (Exception ex)
            {
                // Log error and return a safe default
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return "General";
            }
        }
    }
}
```

### 2. Multi-Tenant Extension

```csharp
using System;
using System.Web;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Portals;
using DotNetNuke.PowerBI.Extensibility;

namespace MyCompany.PowerBI.Extensions
{
    public class MultiTenantRlsExtension : IRlsCustomExtension
    {
        public string GetRlsValue(HttpContext httpContext)
        {
            try
            {
                var userInfo = UserController.Instance.GetCurrentUserInfo();
                var portalSettings = PortalController.Instance.GetCurrentPortalSettings();
                
                // Combine portal and user info for multi-tenant scenario
                var tenantId = portalSettings.PortalAlias.HTTPAlias.Replace(".", "_");
                var userId = userInfo.UserID.ToString();
                
                return $"{tenantId}_{userId}";
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                throw new Exception("Unable to determine tenant RLS value", ex);
            }
        }
    }
}
```

### 3. External System Integration Extension

> **Note**: This example shows integration with external systems. DNN user roles are always passed to Power BI automatically, so you don't need to implement custom extensions just to pass roles.

```csharp
using System;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Users;
using DotNetNuke.PowerBI.Extensibility;

namespace MyCompany.PowerBI.Extensions
{
    public class ExternalSystemRlsExtension : IRlsCustomExtension
    {
        public string GetRlsValue(HttpContext httpContext)
        {
            try
            {
                var userInfo = UserController.Instance.GetCurrentUserInfo();
                
                // Get user's employee ID from external HR system
                var employeeId = GetEmployeeIdFromHR(userInfo.Email);
                
                // Get user's cost center from external ERP system
                var costCenter = GetCostCenterFromERP(employeeId);
                
                // Return a composite identifier for complex filtering
                return $"{employeeId}|{costCenter}";
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return "UNKNOWN|GENERAL";
            }
        }
        
        private string GetEmployeeIdFromHR(string email)
        {
            // Implementation to query HR system
            // This is a simplified example
            return "EMP123";
        }
        
        private string GetCostCenterFromERP(string employeeId)
        {
            // Implementation to query ERP system
            // This is a simplified example
            return "CC001";
        }
    }
}
```

### 4. Database-Driven Extension

```csharp
using System;
using System.Data.SqlClient;
using System.Web;
using DotNetNuke.Entities.Users;
using DotNetNuke.Common.Utilities;
using DotNetNuke.PowerBI.Extensibility;

namespace MyCompany.PowerBI.Extensions
{
    public class DatabaseRlsExtension : IRlsCustomExtension
    {
        public string GetRlsValue(HttpContext httpContext)
        {
            try
            {
                var userInfo = UserController.Instance.GetCurrentUserInfo();
                var connectionString = Config.GetConnectionString();
                
                // Query custom table for user's business unit
                var sql = @"SELECT BusinessUnit FROM CustomUserMapping WHERE DnnUserId = @UserId";
                
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userInfo.UserID);
                        connection.Open();
                        
                        var result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return result.ToString();
                        }
                    }
                }
                
                // Default fallback
                return "DefaultUnit";
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return "DefaultUnit";
            }
        }
    }
}
```

## Deployment Steps

1. **Compile your extension**: Build your project and ensure all dependencies are included.

2. **Deploy to DNN**: Copy your compiled assembly to the DNN `bin` folder or install via DNN package.

3. **Configure in module settings**: 
   - Select "Custom Extension Library" as the User Property Method
   - Enter the fully qualified type name: `MyCompany.PowerBI.Extensions.DepartmentRlsExtension, MyCompany.PowerBI.Extensions`

4. **Test**: Verify that the extension works correctly by checking the RLS values passed to Power BI.

## Best Practices

### Error Handling
- Always implement proper error handling
- Log exceptions using DNN's logging framework
- Provide meaningful fallback values when possible

### Performance
- Keep the `GetRlsValue` method as lightweight as possible
- Consider caching expensive operations
- Avoid blocking operations if possible

### Security
- Validate user permissions before returning sensitive identifiers
- Never expose sensitive information in the RLS value
- Consider the security implications of your RLS logic

### Testing
- Test with different user scenarios
- Verify that RLS filtering works correctly in Power BI
- Test error conditions and fallback scenarios

## Common Patterns

### Caching Pattern
```csharp
private static readonly Dictionary<int, string> _userCache = new Dictionary<int, string>();
private static readonly object _lockObject = new object();

public string GetRlsValue(HttpContext httpContext)
{
    var userInfo = UserController.Instance.GetCurrentUserInfo();
    
    lock (_lockObject)
    {
        if (_userCache.ContainsKey(userInfo.UserID))
        {
            return _userCache[userInfo.UserID];
        }
        
        // Expensive operation here
        var rlsValue = CalculateRlsValue(userInfo);
        _userCache[userInfo.UserID] = rlsValue;
        
        return rlsValue;
    }
}
```

### Configuration Pattern
```csharp
public class ConfigurableRlsExtension : IRlsCustomExtension
{
    private readonly string _configKey;
    
    public ConfigurableRlsExtension()
    {
        _configKey = ConfigurationManager.AppSettings["PowerBI.RLS.ConfigKey"] ?? "Department";
    }
    
    public string GetRlsValue(HttpContext httpContext)
    {
        var userInfo = UserController.Instance.GetCurrentUserInfo();
        return userInfo.Profile.GetProperty(_configKey)?.PropertyValue ?? "Default";
    }
}
```

## Troubleshooting

1. **Assembly Loading Issues**: Ensure your assembly is in the DNN bin folder and all dependencies are available.

2. **Type Not Found**: Verify the fully qualified type name is correct and the assembly is properly deployed.

3. **Runtime Errors**: Check the DNN Event Log for detailed error messages.

4. **RLS Not Working**: Verify that the returned value matches what's expected in your Power BI dataset.

For more information, see the main [RLS Configuration Guide](RLS-Configuration.md).