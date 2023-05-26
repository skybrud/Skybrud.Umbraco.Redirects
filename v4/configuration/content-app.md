# Content App

The package features a content app, which by default is added to all content and media types, but not element types. If you wish to change the behavior of the content app, you can do so either `appsettings.json` or via your own code in C#.


## App Settings

The `Skybrud:Redirects` section defines the overall settings for the redirects package, and the `ContentApp` sub section features various settings for the content app. Eg. as shown here:

```json
{
  "Skybrud": {
    "Redirects": {
      "ContentApp": {
        "Enabled": true,
        "Show": [
          "-content/site",
          "-content/subSite",
          "-media/*",
          "+media/Image"
        ]
      }
    }
  }
}
```

### Enabled

The content app is enabled by default. To disable it, set the `Enabled` property to `false`.

### Show

If you still wish to show the content app, but instead show or hide it for specific content or media types, you can do so via the `Show` property. The value should be an array of string values - eg. `-media/*` to disable the content app for all media types, or `-content/site` to disable the content app for the content type with the alias `site`.

More specific rules take precedence, so even though `-media/*` hides the content app for all media types, `+media/Image` will show it for images specifically.






## Via C#

In C#, the content app is controlled by the package's `RedirectsBackOfficeHelper` service class and it's `GetContentAppFor` method. The default implementation reads from the settings in the `appsettings.json` file.

If you wish to change the default behavior, you can [add your own **backoffice helper** class](/skybrud.umbraco.redirects/docs/v3/configuration/#backoffice-helper) and override the default implementation for the `GetContentAppFor` method:

```csharp
public override ContentApp? GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups) {

    switch (source) {

        case IContent content:

            // Disable the content app for pages that don't already have a template
            if (content.TemplateId == null) return null;

            // Disable the content app for pages that can't have a template
            if (content.ContentType.DefaultTemplate == null) return null;

            // Disable the content app for a specific content type
            if (content.ContentType.Alias == "thatContentTypeWithNoProperties") return null;

            // Default behavior
            return base.GetContentAppFor(source, userGroups);

        default:
            return base.GetContentAppFor(source, userGroups);

    }

}
```

This example checks the `IContent` instance has a template, or it matches a fictious content type alias. If none of those criteria are matched, the base method is used as fallback.
