# Role Level Security (RLS) Configuration Guide

## Overview

Role Level Security (RLS) in the DNN Power BI Embedded module allows you to control data access at the user level by passing user-specific identifiers to Power BI reports. This ensures that users only see data relevant to their roles, departments, or specific business context.

RLS works by passing a user identifier from DNN to Power BI, which Power BI then uses to filter data according to the security rules defined in your Power BI dataset.

## RLS Options

The module provides five different methods to determine the user identifier passed to Power BI:

### 1. Username
**Description**: Uses the DNN username directly as the RLS identifier.

**Use Case**: When your Power BI dataset filters are based on DNN usernames.

**Configuration**:
- Select "Username" from the User Property dropdown
- No additional configuration required

**Example**: If the user's DNN username is "john.doe", this value will be passed to Power BI.

### 2. Email
**Description**: Uses the user's email address as the RLS identifier.

**Use Case**: When your Power BI dataset filters are based on email addresses, which is common in business scenarios.

**Configuration**:
- Select "Email" from the User Property dropdown
- No additional configuration required

**Example**: If the user's email is "john.doe@company.com", this value will be passed to Power BI.

### 3. PowerBiGroup
**Description**: Uses a custom DNN profile property called "PowerBiGroup" as the RLS identifier.

**Use Case**: When you need to group users by business units, departments, or regions that don't directly correspond to DNN roles.

**Configuration**:
- Select "PowerBiGroup" from the User Property dropdown
- Ensure users have the "PowerBiGroup" profile property populated in their DNN profile
- No additional configuration required

**Setup Requirements**:
1. Create a custom profile property named "PowerBiGroup" in DNN
2. Populate this property for all users who need access to Power BI reports
3. Configure your Power BI dataset to filter based on these group values

**Example**: If a user's PowerBiGroup property is set to "Sales-North", this value will be passed to Power BI.

### 4. Custom User Profile Property
**Description**: Uses a flexible template system to create custom RLS identifiers using DNN profile properties.

**Use Case**: When you need complex user identifiers or want to combine multiple profile properties.

**Configuration**:
- Select "Custom User Profile Property" from the User Property dropdown
- In the "Custom user profile property" field, enter your template using placeholders

**Template Syntax**: Use `[PROFILE:PropertyName]` to reference profile properties.

**Examples**:
- `[PROFILE:Department]` - Uses the Department profile property
- `[PROFILE:FirstName].[PROFILE:LastName]` - Combines first and last name
- `[PROFILE:Company]-[PROFILE:Department]` - Creates a composite identifier

**Setup Requirements**:
1. Ensure the referenced profile properties exist in DNN
2. Populate these properties for all users
3. Configure your Power BI dataset to filter based on the resulting values

### 5. Custom Extension Library
**Description**: Allows you to implement custom logic to determine the RLS identifier through a .NET library.

**Use Case**: When you need complex business logic, external data sources, or calculations to determine the user identifier.

**Configuration**:
- Select "Custom Extension Library" from the User Property dropdown
- In the "Custom extension library" field, enter the fully qualified type name of your implementation

**Implementation Requirements**:
1. Create a .NET library that implements the `IRlsCustomExtension` interface
2. Deploy the library to your DNN installation
3. Reference the library using its fully qualified type name

**Interface Definition**:
```csharp
namespace DotNetNuke.PowerBI.Extensibility
{
    public interface IRlsCustomExtension
    {
        string GetRlsValue(HttpContext httpContext);
    }
}
```

**Example Implementation**:
```csharp
public class MyCustomRlsExtension : IRlsCustomExtension
{
    public string GetRlsValue(HttpContext httpContext)
    {
        // Your custom logic here
        // Access to DNN context, user information, database, etc.
        // Return the RLS identifier
        return "custom-identifier";
    }
}
```

**Configuration Value**: `MyNamespace.MyCustomRlsExtension, MyAssembly`

**For detailed implementation examples, see [RLS Custom Extension Examples](RLS-Examples.md)**

## Best Practices

### 1. Data Consistency
- Ensure the identifiers used in RLS match exactly with the values in your Power BI dataset
- Test with different user scenarios to verify proper filtering

### 2. Performance Considerations
- For Custom Extension Library implementations, ensure efficient code as it's called for every report load
- Consider caching strategies for expensive operations

### 3. Security
- Validate that your RLS configuration properly restricts data access
- Test with users from different groups to ensure proper isolation
- Never expose sensitive information in the RLS identifier

### 4. Troubleshooting
- Check the DNN event log for RLS-related errors
- Verify that profile properties are properly populated
- For Custom Extension Library, ensure the library is properly deployed and accessible

## Common Scenarios

### Scenario 1: Department-Based Access
- **RLS Option**: Custom User Profile Property
- **Configuration**: `[PROFILE:Department]`
- **Power BI Setup**: Filter based on department names

### Scenario 2: Multi-Tenant Application
- **RLS Option**: Custom Extension Library
- **Configuration**: Custom logic to determine tenant ID
- **Power BI Setup**: Filter based on tenant identifiers

### Scenario 3: Regional Sales Teams
- **RLS Option**: PowerBiGroup
- **Configuration**: Set PowerBiGroup to region codes (e.g., "US-West", "EU-North")
- **Power BI Setup**: Filter based on regional assignments

## Troubleshooting

### Common Issues

1. **Users see no data**
   - Verify the RLS identifier matches values in your Power BI dataset
   - Check that profile properties are populated
   - Ensure Power BI RLS rules are correctly configured

2. **Users see too much data**
   - Review Power BI RLS rules for proper filtering
   - Verify the correct identifier is being passed

3. **Custom Extension Library errors**
   - Check DNN event log for specific error messages
   - Verify library deployment and assembly references
   - Ensure the interface is properly implemented

### Debugging Steps

1. Enable DNN debug mode to see detailed error messages
2. Check the browser's developer tools for JavaScript errors
3. Verify user profile properties in DNN admin panel
4. Test with a simple RLS configuration first (e.g., Username or Email)

## Version History

- **Version 1.0.16**: Added Custom RLS settings with Username, PowerBiGroup, and Custom options
- **Version 1.0.23**: Added RLS extensibility library support
- **Latest**: Enhanced RLS functionality with improved error handling and caching

For more information, see the [project's release notes](../src/DotNetNuke.PowerBI/ReleaseNotes.txt).