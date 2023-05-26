---
title: FAQ
---

# FAQ

## Why doesn't my redirects work?

Skybrud Redirects adds a ASP.NET Core middleware that executes at the end of the request pipeline. The middleware only looks for redirects if the response in this state has a <code>404 Not Found</code>.

A typical issue is that a custom 404 page has been configured in Umbraco, but instead of <code>404 Not Found</code>, it incorrectly returns a <code>200 OK</code> status code instead. This means that the redirects midlleware doesn't look for redirects.

The best way to validate the status code is to access a non-existing URL on your site and confirm that the returned status code is in fact <code>404 Not Found</code>. The screenshot below is from the **Network** tab in Chrome's **Developer Tools**.

![image](https://user-images.githubusercontent.com/3634580/227796063-63d928e4-e818-4a31-85fd-0910d6d18fc4.png)