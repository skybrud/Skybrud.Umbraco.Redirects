# Skybrud Redirects [![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) [![NuGet](https://img.shields.io/nuget/vpre/Skybrud.Umbraco.Redirects.svg)](https://www.nuget.org/packages/Skybrud.Umbraco.Redirects/4.0.0-alpha002) [![NuGet](https://img.shields.io/nuget/dt/Skybrud.Umbraco.Redirects.svg)](https://www.nuget.org/packages/Skybrud.Umbraco.Redirects) [![Our Umbraco](https://img.shields.io/badge/our-umbraco-%233544B1)](https://our.umbraco.com/packages/website-utilities/skybrud-redirects/)

**Skybrud.Umbraco.Redirects** is a redirects manager for Umbraco 9. The package features a dashboard and property editor that let's users manage inbound redirects from within the Umbraco backoffice.

URLs can be added to redirect to either a content item, media item or a custom URL.

## Installation

The Umbraco 10 version of this package is only available via [NuGet](https://www.nuget.org/packages/Skybrud.Umbraco.Redirects/4.0.0-alpha002). To install the package, you can use either .NET CLI:

```
dotnet add package Skybrud.Umbraco.Redirects --version 4.0.0-alpha002
```

or the older NuGet Package Manager:

```
Install-Package Skybrud.Umbraco.Redirects -Version 4.0.0-alpha002
```

**Umbraco 9**  
For the Umbraco 8 version of this package, see the [**v3/main**](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v3/main) branch instead.

**Umbraco 8**  
For the Umbraco 8 version of this package, see the [**v2/main**](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v2/main) branch instead.

**Umbraco 7**  
For the Umbraco 7 version of this package, see the [**v1/main**](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v1/main) branch instead.






## Features

- Global dashboard for listing all redirects. Supports filtering and searching.

- Property editor that can be added to either a content item or media item to show inbound redirects

- Package only handles custom redirecs - eg. added manually by an editor. The package will let Umbraco 7.5+ handle redirects for renamed pages

- Includes a `RedirectsService` for managing the redirects from your own code

[NuGetPackage]: https://www.nuget.org/packages/Skybrud.Umbraco.Redirects
[GitHubRelease]: https://github.com/skybrud/Skybrud.Umbraco.Redirects/releases






## Documentation

- [Go to the documentation on **packages.skybrud.dk**](https://packages.skybrud.dk/skybrud.umbraco.redirects/docs/v4/)
