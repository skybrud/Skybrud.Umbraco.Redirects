[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) [![NuGet](https://img.shields.io/nuget/vpre/Skybrud.Umbraco.Redirects.svg)](https://www.nuget.org/packages/Skybrud.Umbraco.Redirects) [![NuGet](https://img.shields.io/nuget/dt/Skybrud.Umbraco.Redirects.svg)](https://www.nuget.org/packages/Skybrud.Umbraco.Redirects) [![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-%233544B1)](https://marketplace.umbraco.com/package/skybrud.umbraco.redirects)

**Skybrud.Umbraco.Redirects** is a redirects manager for Umbraco. The package features a dashboard and property editor that let's users manage inbound redirects from within the Umbraco backoffice.

URLs can be added to redirect to either a content item, media item or a custom URL.

## Features

- Global dashboard for listing all redirects. Supports filtering and searching.

- Property editor that can be added to either a content item or media item to show inbound redirects

- Package only handles custom redirects - eg. added manually by an editor. The package will let Umbraco handle redirects for renamed and moved pages

- Includes an `IRedirectsService` for managing the redirects from your own code
