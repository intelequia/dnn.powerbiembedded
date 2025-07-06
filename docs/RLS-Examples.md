# Custom RLS Extension Examples

This document provides examples of how to implement custom RLS (Role Level Security) extensions for the DNN Power BI Embedded module.

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

### 3. Role-Based Extension

```csharp
using System;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.PowerBI.Extensibility;

namespace MyCompany.PowerBI.Extensions
{
    public class RoleBasedRlsExtension : IRlsCustomExtension
    {
        public string GetRlsValue(HttpContext httpContext)
        {
            try
            {
                var userInfo = UserController.Instance.GetCurrentUserInfo();
                var roleController = new RoleController();
                
                // Get user's roles
                var userRoles = roleController.GetUserRoles(userInfo, true);
                
                // Define priority order for roles (highest priority first)
                var priorityRoles = new[] { "Administrators", "Managers", "Supervisors", "Employees" };
                
                // Return the highest priority role
                foreach (var priorityRole in priorityRoles)
                {
                    if (userRoles.Any(r => r.RoleName.Equals(priorityRole, StringComparison.OrdinalIgnoreCase)))
                    {
                        return priorityRole;
                    }
                }
                
                // Default role if no priority roles found
                return "Employees";
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return "Employees";
            }
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