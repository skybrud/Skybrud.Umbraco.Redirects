---
title: Configuration
---

# Configuration

## Backoffice Helper

The package internally uses the `RedirectsBackOfficeHelper` class for various task throughout the backoffice. The class contains a number of virtual methods, meaning you can override these if you replace the DI instance with your own class extending the `RedirectsBackOfficeHelper` class - eg. via a composer:

```csharp
using Skybrud.Umbraco.Redirects.Composers;
using Skybrud.Umbraco.Redirects.Helpers;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace UmbracoNineTests.Features.Redirects
{
    
    [ComposeAfter(typeof(RedirectsComposer))]
    public class MyRedirectsComposer : IComposer
    {

        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<RedirectsBackOfficeHelper, MyRedirectsBackOfficeHelper>();
        }

    }

}
```

And the your custom backoffice helper implementation:

```csharp
using Skybrud.Umbraco.Redirects.Helpers;

namespace UmbracoNineTests.Features.Redirects
{
    
    public class MyRedirectsBackOfficeHelper : RedirectsBackOfficeHelper
    {
        
        public MyRedirectsBackOfficeHelper(RedirectsBackOfficeHelperDependencies dependencies) : base(dependencies)
        {
        }

    }

}
```

Notice that the dependencies has been encapsulated in a single `RedirectsBackOfficeHelperDependencies` instance. This should hopefully avoid breaking changes should the `RedirectsBackOfficeHelper` class need a new dependency in the future.