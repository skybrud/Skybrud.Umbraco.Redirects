# Dashboard

The main component of the package UI-wise is the dashboard where editors can see all existing redirects, as well as creating, editing and deleting redirects.

By default the dashboard is visible to all editors, but if you'd like to restrict access to the dashboard, you can [add your own **backoffice helper** class](/skybrud.umbraco.redirects/docs/v3/configuration/#backoffice-helper), and then override the `GetDashboardAccessRules` method.

The example below shows how `GetDashboardAccessRules` method is overriden to only grant administrators access to the dashboard:

```csharp
public override IAccessRule[] GetDashboardAccessRules() {
    return new IAccessRule[] {
        new AccessRule {
            Type = AccessRuleType.Grant,
            Value = "admin"
        }
    };
}
```