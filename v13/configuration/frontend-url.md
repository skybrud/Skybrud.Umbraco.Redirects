# Frontend URL

The redirects package shows an **Original URL** column in the dashboard. By default, the package will assume that the frontend is hosted on the same domain as the backend. But for instance when using Umbraco as a headless CMS, this might not be the case.

As of `v13.0.6`, the package has improved the logic for determining the frontend URL. For site-specific redirects, this means that the package will try to get the domain configured for the root node. If determining this fails, or the redirect is a global redirect, the package will look for the `Skybrud:Redirects:FeedbackUrl` setting. In your `appsettings.json` file, this would look like this:

```json
{
  "Skybrud": {
    "Redirects": {
      "FrontendUrl": "https://frontend.omgbacon.dk"
    }
  }
}
```

This can also be set in C# by creating your custom composer by implementing Umbraco's `IComposer` interface and using the `PostConfigureRedirects` method:

```csharp
using Skybrud.Umbraco.Redirects.Extensions;
using Umbraco.Cms.Core.Composing;

namespace Umbraco13.Packages.Redirects;

public class MyRedirectsComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {

        builder.Services.PostConfigureRedirects(o => {
            o.FrontendUrl = "https://frontend.omgbacon.dk";
        });

    }

}
```

or if you need to handle more complex scenarios, you can use a custom class instead, which supports injecting dependencies via the constructor:

```csharp
using Microsoft.Extensions.Options;
using Skybrud.Umbraco.Redirects.Config;
using Skybrud.Umbraco.Redirects.Extensions;
using Umbraco.Cms.Core.Composing;

namespace Umbraco13.Packages.Redirects;

public class MyRedirectsComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {
        builder.Services.PostConfigureRedirects<MyConfigureRedirectsSettingsOptions>();
    }

}

public class MyConfigureRedirectsSettingsOptions : IPostConfigureOptions<RedirectsSettings> {

    private readonly IOptions<MySettings> _settings;

    public MyConfigureRedirectsSettingsOptions(IOptions<MySettings> settings) {
        _settings = settings;
    }

    public void PostConfigure(string? name, RedirectsSettings options) {

        if (!string.IsNullOrWhiteSpace(options.FrontendUrl)) return;
        if (string.IsNullOrWhiteSpace(_settings.Value.FrontendDomain)) return;

        options.FrontendUrl = "https://" + _settings.Value.FrontendDomain;

    }

}
```