Skybrud.Umbraco.Redirects
=========================

## Installation

1. [**NuGet Package**][NuGetPackage]  
Install this NuGet package in your Visual Studio project. Makes updating easy.

1. [**ZIP file**][GitHubRelease]  
Grab a ZIP file of the latest release; unzip and move the contents to the root directory of your web application.

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

This step is optinal, but if you wish to show redirects at a global level, you can add a *Redirects* tab to the dashboard of the *Content* section. To do this, open up `~/Config/Dashboard.config`, and search for the line with `<section alias="StartupDashboardSection">`. This particular `<section>` element describes the tabs of the *Content* section. THe *Redirects* tab can be added by adding the following XML as a child element:

```xml
<tab caption="Redirects">
	<access>
		<grant>admin</grant>
	</access>
	<control showOnce="true" addPanel="true" panelCaption="">/App_Plugins/Skybrud.Umbraco.Redirects/Views/Dashboard.html</control>
</tab>
```

With the example above, the tab will only be visible to admins. If you remove the `<access>` element, all your users will be able to see the tab.
