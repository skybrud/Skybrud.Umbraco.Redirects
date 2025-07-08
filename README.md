# Skybrud Redirects

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/skybrud/Skybrud.Umbraco.Redirects/blob/v13/main/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/Skybrud.Umbraco.Redirects.svg)](https://www.nuget.org/packages/Skybrud.Umbraco.Redirects)
[![NuGet](https://img.shields.io/nuget/dt/Skybrud.Umbraco.Redirects.svg)](https://www.nuget.org/packages/Skybrud.Umbraco.Redirects)
[![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-%233544B1)](https://marketplace.umbraco.com/package/skybrud.umbraco.redirects)
[![Skybrud.Umbraco.Redirects at packages.limbo.works](https://img.shields.io/badge/limbo-packages-blue)](https://packages.limbo.works/skybrud.umbraco.redirects/)

**Skybrud.Umbraco.Redirects** is a redirects manager for Umbraco. The package features a dashboard and property editor that let's users manage inbound redirects from within the Umbraco backoffice.

URLs can be added to redirect to either a content item, media item or a custom URL.

<table>
  <tr>
    <td><strong>License:</strong></td>
    <td><a href="https://github.com/skybrud/Skybrud.Umbraco.Redirects/blob/v13/main/LICENSE.md"><strong>MIT License</strong></a></td>
  </tr>
  <tr>
    <td><strong>Umbraco:</strong></td>
    <td>
      Umbraco 13
    </td>
  </tr>
  <tr>
    <td><strong>Target Framework:</strong></td>
    <td>
      .NET 8
    </td>
  </tr>
</table>





<br /><br />

## Installation

### Umbraco 13

`v13.x` of the package targets Umbraco 13 and is available via [**NuGet**](https://www.nuget.org/packages/Skybrud.Umbraco.Redirects/13.0.8). To install the package, you can use either the .NET CLI:

```
dotnet add package Skybrud.Umbraco.Redirects --version 13.0.8
```

or the NuGet Package Manager:

```
Install-Package Skybrud.Umbraco.Redirects -Version 13.0.8
```

### Other versions of Umbraco

- [**`v16/main`**](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v16/main) Umbraco 16
- [**`v15/main`**](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v15/main) Umbraco 15
- ~~[**`v14/main`**](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v14/main) Umbraco 14~~ <sub title="Umbraco 14 has reached end-of-life"><sup>(EOL)</sup></sub>
- ~~[**`v4/main`**](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v4/main) Umbraco 10, 11 and 12~~ <sub title="Umbraco 10, 11 and 12 have reached end-of-life"><sup>(EOL)</sup></sub>
- ~~[**`v3/main`**](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v3/main) Umbraco 9~~ <sub title="Umbraco 9 has reached end-of-life"><sup>(EOL)</sup></sub>
- ~~[**`v2/main`**](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v2/main) Umbraco 8~~ <sub title="Umbraco 8 has reached end-of-life"><sup>(EOL)</sup></sub>
- ~~[**`v1/main`**](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v1/main) Umbraco 7~~ <sub title="Umbraco 7 has reached end-of-life"><sup>(EOL)</sup></sub>




<br /><br />

## Features

- Global dashboard for listing all redirects. Supports filtering and searching

- Property editor that can be added to either a content item or media item to show inbound redirects

- Package only handles custom redirecs - e.g. added manually by an editor. The package will let Umbraco 7.5+ handle redirects for renamed and moved pages

- Includes a `RedirectsService` for managing the redirects from your own code

[NuGetPackage]: https://www.nuget.org/packages/Skybrud.Umbraco.Redirects
[GitHubRelease]: https://github.com/skybrud/Skybrud.Umbraco.Redirects/releases




<br /><br />

## Add-ons

- [**Skybrud.Umbraco.Redirects.Import**](https://github.com/skybrud/Skybrud.Umbraco.Redirects.Import)  
Add-on for handling imports and exports of redirects supporting formats like CSV, XLSX and JSON.





<br /><br />

## Documentation

- [Go to the documentation on **packages.skybrud.dk**](https://packages.skybrud.dk/skybrud.umbraco.redirects/docs/v13/)
