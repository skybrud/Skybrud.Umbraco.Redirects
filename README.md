Skybrud.Umbraco.Redirects
=========================

[Looking for the Umbraco 7 version of the package?](https://github.com/skybrud/Skybrud.Umbraco.Redirects/tree/dev-v7)

**Skybrud.Umbraco.Redirects** is a redirects manager for Umbraco 8. The package features a dashboard and property editor that let's users manage inbound redirects from within the Umbraco backoffice.

URLs can be added to redirect to either a content item, media item or a custom URL.

## Installation

### Install via NuGet
This is the recommend approach, as you install the NuGet package in your Visual Studio project, and NuGet takes care of the rest.

1. [**NuGet Package**][NuGetPackage]

### Manual install
You can also download a ZIP file of the latest release directly from GitHub, unzip, and move the contents to the root directory of your web application.

1. [**Download ZIP file**][GitHubRelease]  
  Download the ZIP file directly from here on GitHub. The ZIP contains all necessary files to run the package.

2. **Unzip**  
  Unzip and move the contents to the root directory of your web application.

3. **Install HTTP module**  
  The package features a HTTP module. When downloading the ZIP file, you must install this manually. In your root `Web.config` file, search for the `<system.webServer>` element. Then add the following to the `<modules>` child element:
  
     ```xml
    <remove name="RedirectsModule" />
    <add name="RedirectsModule" type="Skybrud.Umbraco.Redirects.Routing.RedirectsModule, Skybrud.Umbraco.Redirects" />
    ```

    The order shouldn't matter that much, but we typically add it right after:

    ```xml
    <remove name="UmbracoModule" />
    <add name="UmbracoModule" type="Umbraco.Web.UmbracoModule,umbraco" />
    ```



## Features

- Global dashboard for listing all redirects. Supports filtering and searching.

- Property editor that can be added to either a content item or media item to show inbound redirects

- Package only handles custom redirecs - eg. added manually by an editor. The will let Umbraco 7.5+ handle redirects for renamed pages

- Includes a `RedirectsRepository` for managing the redirects from your own code

[NuGetPackage]: https://www.nuget.org/packages/Skybrud.Umbraco.Redirects
[GitHubRelease]: https://github.com/skybrud/Skybrud.Umbraco.Redirects



## Screenshots

Besides the dashboard shown in the top of this page, the package also features property editor that let's users add new inbound redirect directly from the content or media item being editied.

For instance the screenshot below illustrates the property editor added to a content type:

![image](https://cloud.githubusercontent.com/assets/3634580/22441953/c3c374fa-e739-11e6-8453-78402e3103fd.png)

Or the same property editor added to a media type - eg. here added to the `Image` media type:

<!--![image](https://cloud.githubusercontent.com/assets/3634580/22441813/3b8045a0-e739-11e6-9182-8011cc9785fb.png)-->
![image](https://cloud.githubusercontent.com/assets/3634580/22441900/8810a022-e739-11e6-858b-4c62d86796ad.png)

## Under the hood

The package comes with a HTTP module that will kick in when Umbraco or IIS returns a response with a 404 status code. If this is the case, the module will look up the requested URL, and then redirect the user if a matching redirect is found in the database.
