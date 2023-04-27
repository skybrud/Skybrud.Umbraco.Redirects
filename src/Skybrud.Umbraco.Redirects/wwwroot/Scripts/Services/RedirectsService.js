angular.module("umbraco.services").factory("skybrudRedirectsService", function ($http, editorService, localizationService, notificationsService, overlayService) {

    // Get the cache buster value
    const cacheBuster = Umbraco.Sys.ServerVariables.skybrud.redirects.cacheBuster;

    // Get the base URL for the API controller
    const baseUrl = Umbraco.Sys.ServerVariables.skybrud.redirects.baseUrl;

    const service = {

        toUmbracoLink: function(e) {

            if (!e) return null;

            // Create a copy of "e" so we don't modify the original value
            const link = Utilities.copy(e);

            // Make sure to set the UDI
            switch (link.type) {
                case "document":
                case "content":
                    link.udi = `umb://document/${link.key}`;
                    break;
                case "media":
                    link.udi = `umb://media/${link.key}`;
                    break;
            }

            // For legacy values, the fragment might be stored as part of the
            // URL. If so, we need to isolate it in it's own property
            if (!link.fragment) {
                const pos = link.url.indexOf("#");
                if (pos > 0) {
                    link.fragment = link.url.substr(pos + 1);
                    link.url = link.url.substr(0, pos);
                }
            }

            // For legacy values, the query string might be stored as part of the
            // URL. If so, we need to isolate it in it's own property
            if (!link.query) {
                const pos = link.url.indexOf("?");
                if (pos > 0) {
                    link.query = link.url.substr(pos + 1);
                    link.url = link.url.substr(0, pos);
                }
            }

            // Append the query and fragment to the anchor value (if specified)
            link.anchor = "";
            if (link.query) link.anchor = link.query;
            if (link.fragment) link.anchor += link.fragment;

            return link;

        },

        parseUmbracoLink: function (e) {

            let url = e.url;
            let query = null;
            let fragment = null;

            // Isolate the fragment if specified in the URL
            const pos1 = url.indexOf("#");
            if (pos1 >= 0) {
                fragment = url.substr(pos1);
                url = url.substr(0, pos1);
            }

            // Isolate the query string if specified in the URL
            const pos2 = url.indexOf("?");
            if (pos2 >= 0) {
                query = url.substr(pos2 + 1);
                url = url.substr(0, pos2);
            }

            // Parse the "anchor" value
            if (e.anchor) {

                // Isolate the fragment if specified in the "anchor" value (overwrites fragment from the URL)
                const pos3 = e.anchor.indexOf("#");
                if (pos3 >= 0) {
                    fragment = e.anchor.substr(pos3);
                    e.anchor = e.anchor.substr(0, pos3);
                }

                // Treat remaining anchor value as query string (append if URL also has query string)
                if (e.anchor) {
                    if (e.anchor.indexOf("?") === 0 || e.anchor.indexOf("&") === 0) {
                        query = (query ? query + "&" : "") + e.anchor.substr(1);
                    } else {
                        query = (query ? query + "&" : "") + e.anchor;
                    }
                }

            }

            let key = "00000000-0000-0000-0000-000000000000";
            let type = "url";

            if (e.udi) {
                key = e.udi.split("/")[3];
                type = e.udi.split("/")[2];
            }

            const link = {
                id: e.id || 0,
                key: key,
                url: url,
                displayUrl: url,
                name: e.name,
                type: type === "document" ? "content" : type
            };

            if (query) {
                link.query = query;
                link.displayUrl += "?" + query;
            }

            if (fragment) {
                link.fragment = fragment;
                link.displayUrl += fragment;
            }

            return link;

        },

        addLink: function (callback) {
            editLink(null, callback);
        },

        editLink: function (link, callback) {

            // Convert our link object to something Umbraco's link picker can understand
            const target = service.toUmbracoLink(link);

            // Open the link picker overlay
            editorService.linkPicker({
                size: "medium",
                currentTarget: target,
                hideTarget: true,
                submit: function (model) {

                    // Make sure the user has picked a valid link
                    if (!model.target.id && !model.target.url && !confirm("The selected link appears to be empty. Do you want to continue anyways?")) return;

                    // Convert the selected link back to our own format
                    const newLink = service.parseUmbracoLink(model.target);

                    // Invoke the callback (if specified)
                    if (callback) callback(newLink);

                },
                close: function () {
                    editorService.close();
                }
            });

        },

        addRedirect: function (options) {

            if (!options) options = {};
            if (typeof options === "function") options = { callback: options };

            const o = {
                size: "medium",
                view: `/App_Plugins/Skybrud.Umbraco.Redirects/Views/Dialogs/Redirect.html?v=${cacheBuster}`,
                options: options,
                submit: function(value) {
                    if (options.callback) options.callback(value);
                    editorService.close();
                },
                close: function() {
                    editorService.close();
                }
            };

            if (options.destination) o.destination = options.destination;

            editorService.open(o);

        },

        editRedirect: function (redirect, options) {

            if (!options) options = {};
            if (typeof options === "function") options = { callback: options };

            editorService.open({
	            size: "medium",
                view: `/App_Plugins/Skybrud.Umbraco.Redirects/Views/Dialogs/Redirect.html?v=${cacheBuster}`,
                redirect: redirect,
                options: options,
                submit: function (value) {
                    if (options.callback) options.callback(value);
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });

        },

        deleteRedirect: function (redirect, success, failed) {
            $http({
                method: "GET",
                url: `${baseUrl}DeleteRedirect`,
                params: {
                    redirectId: redirect.key
                }
            }).then(function () {
                notificationsService.success("Redirect deleted", "Your redirect was successfully deleted.");
                if (success) success(redirect);
            }, function (res) {
                notificationsService.error("Deleting redirect failed", res && res.data && res.data.meta ? res.data.meta.error : "The server was unable to delete your redirect.");
                if (failed) failed(redirect);
            });
        },

        requestDeleteRedirect: function(options) {

            if (!options) options = {};
            if (!options.redirect) return;

            if (typeof options.submit !== "function") {
                options.submit = function() {
                    overlayService.close();
                };
            }

            if (typeof options.close !== "function") {
                options.close = function() {
                    overlayService.close();
                };
            }

            const overlay = {
                title: "Confirm delete",
                content: `Are you sure you want to delete the redirect at <strong>${options.redirect.destination.displayUrl}</strong> ?`,
                submit: function() {

                    // Update the button state in the UI
                    overlay.submitButtonState = "busy";

                    // Delete the redirect
                    service.deleteRedirect(options.redirect, function () {
                        if (typeof options.submit === "function") {
                            options.submit(overlay);
                        } else {
                            overlayService.close();
                        }
                    }, function () {
                        overlay.submitButtonState = "error";
                    });

                },
                close: function() {
                    options.close(overlay);
                }
            };

            localizationService.localize("redirects_overlayDeleteTitle", null, overlay.title).then(function (value) {
                overlay.title = value;
            });

            localizationService.localize("redirects_overlayDeleteMessage", [options.redirect.destination.displayUrl], overlay.content).then(function (value) {
                overlay.content = value;
            });

            // Open the overlay
            overlayService.confirmDelete(overlay);

        },

        isValidUrl: function (url) {

            // Make sure we have a string and trim all leading and trailing whitespace
            url = $.trim(url + "");

            // For now a valid URL should start with a forward slash
            return url.indexOf("/") === 0;

        },

        propertiesToObject: function (array) {

            var result = {};

            array.forEach(function (p) {
                result[p.alias] = p.value === undefined ? null : p.value;
            });

            return result;

        },

        getCulturesByNodeId: function (id) {
            return $http.get(`${baseUrl}GetCultures?id=${id}`);
        }

    };

    service.getRootNodes = function () {
        return $http.get(`${baseUrl}GetRootNodes`);
    };

    return service;

});