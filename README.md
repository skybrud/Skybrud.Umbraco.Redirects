Skybrud.Umbraco.Redirects
=========================

This package features a dashboard and property editor that let's users manage inbound redirects from within the Umbraco backoffice.

URLs can be added to redirect to either a content item, media item or a custom URL.

![image](https://cloud.githubusercontent.com/assets/3634580/22441437/ac65dc6e-e737-11e6-8a5c-e89a46aea3a1.png)

## Installation

1. [**NuGet Package**][NuGetPackage]  
Install this NuGet package in your Visual Studio project. Makes updating easy.

1. [**ZIP file**][GitHubRelease]  
Grab a ZIP file of the latest release; unzip and move the contents to the root directory of your web application.

## Features

- Global dashboard for listing all redirects. Supports filtering and searching.

- Property editor that can be added to either a content item or media item to show inbound redirects

- Package only handles custom redirecs - eg. added manually by an editor. The will let Umbraco 7.5+ handle redirects for renamed pages

- Includes a `RedirectsRepository` for managing the redirects from your own code

[NuGetPackage]: https://www.nuget.org/packages/Skybrud.Umbraco.Redirects
[GitHubRelease]: https://github.com/skybrud/Skybrud.Umbraco.Redirects

## Configuration

#### Adding the HTTP module to Web.config

In order to handle the redirects, this package comes with a HTTP module - which for now must be added manuelly to `Web.config`. In your `Web.config`, search for the `<system.webServer>` element. Then add the following to the `<modules>` child element.

```xml
<remove name="RedirectsHttpModule" />
<add name="RedirectsHttpModule" type="Skybrud.Umbraco.Redirects.Routing.RedirectsHttpModule, Skybrud.Umbraco.Redirects" />
```

The order shouldn't matter that much, but we typically add it right after:

```xml
<remove name="UmbracoModule" />
<add name="UmbracoModule" type="Umbraco.Web.UmbracoModule,umbraco" />
```

In a standard Umbraco 7.5, the entire `<modules>` element would then look like:

```xml
<modules runAllManagedModulesForAllRequests="true">
	<remove name="WebDAVModule" />

	<remove name="UrlRewriteModule" />
	<add name="UrlRewriteModule" type="UrlRewritingNet.Web.UrlRewriteModule, UrlRewritingNet.UrlRewriter" />

	<remove name="UmbracoModule" />
	<add name="UmbracoModule" type="Umbraco.Web.UmbracoModule,umbraco" />

	<remove name="RedirectsHttpModule" />
	<add name="RedirectsHttpModule" type="Skybrud.Umbraco.Redirects.Routing.RedirectsHttpModule, Skybrud.Umbraco.Redirects" />

	<remove name="ScriptModule" />
	<add name="ScriptModule" preCondition="managedHandler" type="System.Web.Handlers.ScriptModule, System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />

	<remove name="ClientDependencyModule" />
	<add name="ClientDependencyModule" type="ClientDependency.Core.Module.ClientDependencyModule, ClientDependency.Core" />

	<!-- Needed for login/membership to work on homepage (as per http://stackoverflow.com/questions/218057/httpcontext-current-session-is-null-when-routing-requests) -->
	<remove name="FormsAuthentication" />
	<add name="FormsAuthentication" type="System.Web.Security.FormsAuthenticationModule" />
	<add name="ImageProcessorModule" type="ImageProcessor.Web.HttpModules.ImageProcessingModule, ImageProcessor.Web" />
</modules>
```

#### Adding a Redirects tab to the Content dashboard

This step is optional, but if you wish to show redirects at a global level, you can add a *Redirects* tab to the dashboard of the *Content* section. To do this, open up `~/Config/Dashboard.config`, and search for the line with `<section alias="StartupDashboardSection">`. This particular `<section>` element describes the tabs of the *Content* section. THe *Redirects* tab can be added by adding the following XML as a child element:

```xml
<tab caption="Redirects">
	<access>
		<grant>admin</grant>
	</access>
	<control showOnce="true" addPanel="true" panelCaption="">/App_Plugins/Skybrud.Umbraco.Redirects/Views/Dashboard.html</control>
</tab>
```

With the example above, the tab will only be visible to admins. If you remove the `<access>` element, all your users will be able to see the tab.

The dashboard will list all redirects - for content, media and custom URLs.

## Screenshots

Besides the dashboard shown in the top of this page, the package also features property editor that let's users add new inbound redirect directly from the content or media item being editied.

For instance the screenshot below illustrates the property editor added to a content type:

![image](https://cloud.githubusercontent.com/assets/3634580/22441953/c3c374fa-e739-11e6-8453-78402e3103fd.png)

Or the same property editor added to a media type - eg. here added to the `Image` media type:

<!--![image](https://cloud.githubusercontent.com/assets/3634580/22441813/3b8045a0-e739-11e6-9182-8011cc9785fb.png)-->
![image](https://cloud.githubusercontent.com/assets/3634580/22441900/8810a022-e739-11e6-858b-4c62d86796ad.png)

## Under the hood

The package comes with a HTTP module that will kick in when Umbraco or IIS returns a response with a 404 status code. If this is the case, the module will look up the requested URL, and then redirect the user if a matching redirect is found in the database.
