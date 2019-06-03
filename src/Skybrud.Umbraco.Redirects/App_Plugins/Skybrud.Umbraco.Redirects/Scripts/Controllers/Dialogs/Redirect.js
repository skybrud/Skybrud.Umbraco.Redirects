angular.module("umbraco").controller("SkybrudUmbracoRedirects.RedirectDialog.Controller", function ($scope, $http, editorService, notificationsService, skybrudRedirectsService, localizationService, formHelper) {

    $scope.options = $scope.model.options || {};

    $scope.model.title = "Add new redirect";
    localizationService.localize("redirects_addNewRedirect").then(function (value) { $scope.model.title = value; });

    var destionation = null;







    $scope.model.hiddenProperties = [];

    if ($scope.model.redirect) {

        $scope.model.title = "Edit redirect";
        localizationService.localize("redirects_editRedirect").then(function (value) { $scope.model.title = value; });

        $scope.model.submitButtonLabel = "Save";

        destionation = {
            id: $scope.model.redirect.linkId,
            key: $scope.model.redirect.linkKey,
            url: $scope.model.redirect.linkUrl,
            type: $scope.model.redirect.linkMode
        };

        $scope.model.hiddenProperties.push({
            alias: "id",
            value: $scope.model.redirect.id
        });

        $scope.model.hiddenProperties.push({
            alias: "key",
            value: $scope.model.redirect.key
        });

    }

    $scope.model.properties = [
        {
            alias: "originalUrl",
            label: "Original URL",
            description: "Specify the original URL to match from which the user should be redirected to the destination.",
            view: "/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/OriginalUrl.html",
            value: $scope.model.redirect ? $scope.model.redirect.url + ($scope.model.redirect.queryString ? "?" + $scope.model.redirect.queryString : "") : "",
            validation: {
                mandatory: true
            }
        },
        {
            alias: "destination",
            label: "Destination",
            description: "Select the page or URL the user should be redirected to.",
            view: "/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/Destination.html",
            value: destionation,
            validation: {
                mandatory: true
            }
        }
    ];

    $scope.model.advancedProperties = [
        {
            alias: "permanent",
            label: "Redirect type",
            labelKey: "redirects_propertyRedirectTypeName",
            description: "Select the type of the redirect. Notice that browsers will remember permanent redirects.",
            descriptionKey: "redirects_propertyRedirectTypeDescription",
            view: "/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/RadioGroup.html",
            value: $scope.model.redirect ? $scope.model.redirect.permanent : true,
            config: {
                options: [
                    {
                        label: "Permanent",
                        labelKey: "redirects_labelPermanent",
                        value: true
                    },
                    {
                        label: "Temporary",
                        labelKey: "redirects_labelTemporary",
                        value: false
                    }
                ]
            }
        },
        {
            alias: "forward",
            label: "Forward query string",
            labelKey: "redirects_forwardQueryString",
            description: "When enabled, the query string of the original request is forwarded to the redirect location (pass through).",
            descriptionKey: "redirects_forwardQueryStringDescription",
            view: "/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/RadioGroup.html",
            value: $scope.model.redirect ? $scope.model.redirect.forward : false,
            config: {
                options: [
                    {
                        label: "Enabled",
                        labelKey: "redirects_labelEnabled",
                        value: true
                    },
                    {
                        label: "Disabled",
                        labelKey: "redirects_labelDisabled",
                        value: false
                    }
                ]
            }
        }
    ];

    var allProperties = $scope.model.properties.concat($scope.model.advancedProperties);
    allProperties = allProperties.concat($scope.model.hiddenProperties);

    angular.forEach(allProperties, function (p) {
        if (p.labelKey) localizationService.localize(p.labelKey).then(function (value) { p.label = value; });
        if (p.descriptionKey) localizationService.localize(p.descriptionKey).then(function (value) { p.description = value; });
        if (p.config && p.config.options) {
            angular.forEach(p.config.options, function (o) {
                if (o.labelKey) localizationService.localize(o.labelKey).then(function (value) { o.label = value; });
            });
        }
    });

    function initLabels() {

        //localizationService.localize("redirects_allSites").then(function (value) {
        //    $scope.rootNodes[0].name = value;
        //});

        $scope.labels = {
            saveSuccessfulTitle: "Redirect added",
            saveSuccessfulMessage: "Your redirect has successfully been added.",
            errorAddFailedTitle: "Saving failed",
            errorAddFailedMessage: "The redirect could not be saved due to an error on the server."
        };

        angular.forEach($scope.labels, function (value, key) {
            localizationService.localize("redirects_" + key).then(function (value) {
                $scope.labels[key] = value;
            });
        });

    }

    initLabels();

    $scope.save = function () {

        // Map the properties back to an object we can send to the API
        var redirect = skybrudRedirectsService.propertiesToObject(allProperties);

        // Attempt to submit the form (Angular validation will kick in)
        if (!formHelper.submitForm({ scope: $scope })) return;

        // Reset the Angular form
        formHelper.resetForm({ scope: $scope });

        // Make sure we set a loading state
        $scope.loading = true;

        if (redirect.key) {
            $http({
                method: "POST",
                url: "/umbraco/backoffice/api/Redirects/EditRedirect",
                params: {
                    redirectId: redirect.key
                },
                data: redirect
            }).then(function (r) {
                $scope.loading = false;
                notificationsService.success($scope.labels.saveSuccessfulTitle, $scope.labels.saveSuccessfulMessage);
                $scope.model.submit(r);
            }, function (res) {
                $scope.loading = false;
                notificationsService.error($scope.labels.errorAddFailedTitle, res && res.data && res.data.meta ? res.data.meta.error : $scope.labels.errorAddFailedMessage);
            });
        } else {
            $http({
                method: "POST",
                url: "/umbraco/backoffice/api/Redirects/AddRedirect",
                data: redirect
            }).then(function (r) {
                $scope.loading = false;
                notificationsService.success($scope.labels.saveSuccessfulTitle, $scope.labels.saveSuccessfulMessage);
                $scope.model.submit(r);
            }, function (res) {
                $scope.loading = false;
                notificationsService.error($scope.labels.errorAddFailedTitle, res && res.data && res.data.meta ? res.data.meta.error : $scope.labels.errorAddFailedMessage);
            });
        }

    };

    $scope.close = function () {
        if ($scope.model.close) {
            $scope.model.close();
        } else {
            editorService.close();
        }
    };

});