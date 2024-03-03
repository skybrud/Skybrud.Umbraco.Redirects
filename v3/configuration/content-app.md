# Content App

The package features a content app, which by default is added to all content and media types, but not element types.

There may be some content types where the content app isn't relevant to show. The content app is controlled by the `RedirectsBackOfficeHelper` class and it's `GetContentAppFor` method.

If you wish to change the default behavior, you can [add your own **backoffice helper** class](/skybrud.umbraco.redirects/docs/v3/configuration/#backoffice-helper) and override the default implementation for the `GetContentAppFor` method:

```csharp
public override ContentApp GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups) {

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