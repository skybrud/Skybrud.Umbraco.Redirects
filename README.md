# Skybrud Redirects

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/skybrud/Skybrud.Umbraco.Redirects/blob/v4/main/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/Skybrud.Umbraco.Redirects.svg)](https://www.nuget.org/packages/Skybrud.Umbraco.Redirects)
[![NuGet](https://img.shields.io/nuget/dt/Skybrud.Umbraco.Redirects.svg)](https://www.nuget.org/packages/Skybrud.Umbraco.Redirects)
[![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-%233544B1)](https://marketplace.umbraco.com/package/skybrud.umbraco.redirects)

**Skybrud.Umbraco.Redirects** is a redirects manager for Umbraco. The package features a dashboard and property editor that let's users manage inbound redirects from within the Umbraco backoffice.

URLs can be added to redirect to either a content item, media item or a custom URL.

<table>
  <tr>
    <td><strong>License:</strong></td>
    <td><a href="https://github.com/skybrud/Skybrud.Umbraco.Redirects/blob/v4/main/LICENSE.md"><strong>MIT License</strong></a></td>
  </tr>
  <tr>
    <td><strong>Umbraco:</strong></td>
    <td>
      Umbraco 10, 11 and 12
      <sub><sup>(and <a href="https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v3/main">Umbraco 9</a>, <a href="https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v2/main">Umbraco 8</a> and <a href="https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v1/main">Umbraco 7</a>)</sup></sub>
    </td>
  </tr>
  <tr>
    <td><strong>Target Framework:</strong></td>
    <td>
      .NET 6
      <sub><sup>(and <a href="https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v3/main">.NET 5</a>, <a href="https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v2/main">.NET 4.7.2</a>, and <a href="https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v1/main">.NET 4.5</a>)</sup></sub>
    </td>
  </tr>
</table>





<br /><br />

## Installation

**Umbraco 10, 11 and 12**  

Version 4 of this package supports Umbraco versions 10, 11 and 12. The package is only available via [**NuGet**](https://www.nuget.org/packages/Skybrud.Umbraco.Redirects/4.0.20). To install the package, you can use either .NET CLI:

```
dotnet add package Skybrud.Umbraco.Redirects --version 4.0.20
```

or the NuGet Package Manager:

```
Install-Package Skybrud.Umbraco.Redirects -Version 4.0.20
```

**Umbraco 9**  
For the Umbraco 9 version of this package, see the [**v3/main**](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v3/main) branch instead.

**Umbraco 8**  
For the Umbraco 8 version of this package, see the [**v2/main**](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v2/main) branch instead.

**Umbraco 7**  
For the Umbraco 7 version of this package, see the [**v1/main**](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/v1/main) branch instead.




<br /><br />

## Features

- Global dashboard for listing all redirects. Supports filtering and searching.

- Property editor that can be added to either a content item or media item to show inbound redirects

- Package only handles custom redirecs - eg. added manually by an editor. The package will let Umbraco 7.5+ handle redirects for renamed pages

- Includes a `RedirectsService` for managing the redirects from your own code

[NuGetPackage]: https://www.nuget.org/packages/Skybrud.Umbraco.Redirects
[GitHubRelease]: https://github.com/skybrud/Skybrud.Umbraco.Redirects/releases




<br /><br />

## Add-ons

- [**Skybrud.Umbraco.Redirects.Import**](https://github.com/skybrud/Skybrud.Umbraco.Redirects.Import)  
Add-on for handling imports and exports of redirects supporting formats like CSV, XLSX and JSON.





<br /><br />

## Documentation

- [Go to the documentation on **packages.skybrud.dk**](https://packages.skybrud.dk/skybrud.umbraco.redirects/docs/v4/)
