# DNN Power BI Embedded Documentation

This directory contains comprehensive documentation for the DNN Power BI Embedded module.

## Available Documentation

### [RLS Configuration Guide](RLS-Configuration.md)
Complete guide to configuring Role Level Security (RLS) in the Power BI Embedded module. This document covers:
- Overview of RLS functionality
- All 5 RLS configuration options (Username, Email, PowerBiGroup, Custom Profile Properties, Custom Extension Library)
- Configuration steps for each option
- Best practices and troubleshooting

### [RLS Custom Extension Examples](RLS-Examples.md)
Practical examples and code samples for implementing custom RLS extensions. This document includes:
- Complete example implementations for common scenarios
- Department-based, multi-tenant, and role-based extensions
- Database-driven RLS implementations
- Deployment and testing guidance
- Best practices for performance and security

### [Embed Token Renewal](Token-Renewal.md)
Guide to App Owns Data token renewal, including expiration handling, security, deployment, and testing.

## Quick Start

1. **Basic Setup**: Start with the [RLS Configuration Guide](RLS-Configuration.md) to understand the available options
2. **Simple Implementation**: Use Username or Email options for basic scenarios
3. **Advanced Implementation**: Use Custom Profile Properties or Custom Extension Library for complex business logic
4. **Development**: Refer to [RLS Examples](RLS-Examples.md) for code samples and patterns

## Subscription management

Users with subscription permissions can only list and manage subscriptions they created.
Portal administrators and superusers can manage all subscriptions in the portal, subject to the existing report/workspace permissions.
The subscription list and edit form display the creator's display name as the owner.
Editing a subscription does not transfer ownership. If the creator is no longer available, the UI explicitly indicates this.
Ownership is enforced server-side for editing, deleting, saving report changes and running saved subscriptions immediately.

## Dataset refresh statistics

The statistics view counts all workspace datasets except usage metrics models.
Datasets explicitly marked as non-refreshable are excluded from refresh history queries.
If refreshability metadata is missing, the query is still attempted. The specific HTTP 415
`InvalidRequest` response indicating that the API requires a model-based dataset is treated
as not applicable and logged only at debug level, without degrading refresh health.
Other query failures still produce warnings. If no refresh history is available, the
existing no-data indication is preserved.

## Getting Help

- Check the [main project README](../README.md) for general information
- Visit the [project Wiki](https://github.com/intelequia/dnn.powerbiembedded/wiki) for additional documentation
- Review the [release notes](../src/DotNetNuke.PowerBI/ReleaseNotes.txt) for version-specific information

## Contributing

To contribute to this documentation:
1. Follow the existing documentation style and structure
2. Include practical examples where appropriate
3. Update this index when adding new documentation files
4. Ensure all links are working and up-to-date