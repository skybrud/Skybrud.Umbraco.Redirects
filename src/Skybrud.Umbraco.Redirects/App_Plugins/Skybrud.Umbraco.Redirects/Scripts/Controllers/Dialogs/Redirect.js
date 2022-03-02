angular.module("umbraco").controller("SkybrudUmbracoRedirects.RedirectDialog.Controller", function ($scope, $http, editorService, notificationsService, skybrudRedirectsService, localizationService, formHelper) {

    // Get the cache buster value
    const cacheBuster = Umbraco.Sys.ServerVariables.skybrud.redirects.cacheBuster;

    // Get the base URL for the API controller
    const baseUrl = Umbraco.Sys.ServerVariables.skybrud.redirects.baseUrl;

    const vm = this;

    vm.options = $scope.model.options || {};

    $scope.model.submitButtonLabelKey = "redirects_add";

    $scope.model.title = "Add new redirect";
    localizationService.localize("redirects_addNewRedirect").then(function (value) { $scope.model.title = value; });

    let destionation = null;

    $scope.model.hiddenProperties = [];

    if ($scope.model.redirect) {

        $scope.model.title = "Edit redirect";
        localizationService.localize("redirects_editRedirect").then(function (value) { $scope.model.title = value; });

        $scope.model.submitButtonLabelKey = "redirects_save";

        destionation = $scope.model.redirect.destination;

        $scope.model.hiddenProperties.push({
            alias: "id",
            value: $scope.model.redirect.id
        });

        $scope.model.hiddenProperties.push({
            alias: "key",
            value: $scope.model.redirect.key
        });

    } else if ($scope.model.destination) {

        destionation = $scope.model.destination;

    }

    $scope.model.properties = [];

    $scope.model.properties.push({
        alias: "rootNodeId",
        label: "Site",
        labelKey: "redirects_propertySite",
        description: "Specify the site that the original URL to match from belongs to.",
        descriptionKey: "redirects_propertySiteDescription",
        view: `/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/Site.html?v=${cacheBuster}`,
        value: $scope.model.redirect && $scope.model.redirect.rootId ? $scope.model.redirect.rootId : 0,
        config: {
            rootNodes: vm.options.rootNodes
        },
        validation: {
            mandatory: false
        }
    });

    $scope.model.properties.push({
        alias: "originalUrl",
        label: "Original URL",
        labelKey: "redirects_propertyOriginalUrl",
        description: "Specify the original URL to match from which the user should be redirected to the destination.",
        descriptionKey: "redirects_propertyOriginalUrlDescription",
        view: `/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/OriginalUrl.html?v=${cacheBuster}`,
        value: $scope.model.redirect ? $scope.model.redirect.url + ($scope.model.redirect.queryString ? `?${$scope.model.redirect.queryString}` : "") : "",
        validation: {
            mandatory: true
        }
    });

    $scope.model.properties.push({
        alias: "destination",
        label: "Destination",
        labelKey: "redirects_propertyDestination",
        description: "Select the page or URL the user should be redirected to.",
        descriptionKey: "redirects_propertyDestinationDescription",
        view: `/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/Destination.html?v=${cacheBuster}`,
        value: destionation,
        validation: {
            mandatory: true
        }
    });

    $scope.model.advancedProperties = [
        {
            alias: "permanent",
            label: "Redirect type",
            labelKey: "redirects_propertyRedirectTypeName",
            description: "Select the type of the redirect. Notice that browsers will remember permanent redirects.",
            descriptionKey: "redirects_propertyRedirectTypeDescription",
            view: `/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/RadioGroup.html?v=${cacheBuster}`,
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
            view: `/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/RadioGroup.html?v=${cacheBuster}`,
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

    $scope.model.infoProperties = [];

    // We only wish to initialize/show the info properties when we have a redirect (eg. when editing, not adding)
    if ($scope.model.redirect && $scope.model.redirect.id) {
        $scope.model.infoProperties = [
            {
                alias: "id",
                label: "ID",
                labelKey: "redirects_propertyId",
                view: `/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/Code.html?v=${cacheBuster}`,
                value: $scope.model.redirect ? $scope.model.redirect.id : null,
                readonly: true
            },
            {
                alias: "key",
                label: "Key",
                labelKey: "redirects_propertyKey",
                view: `/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/Code.html?v=${cacheBuster}`,
                value: $scope.model.redirect ? $scope.model.redirect.key : null,
                readonly: true
            },
            {
                alias: "createDate",
                label: "Created Date",
                labelKey: "redirects_propertyCreateDate",
                view: `/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/Timestamp.html?v=${cacheBuster}`,
                value: $scope.model.redirect ? $scope.model.redirect.createDate : null,
                hello: moment(new Date($scope.model.redirect.updateDate)).fromNow(),
                readonly: true
            },
            {
                alias: "updateDate",
                label: "Updated Date",
                labelKey: "redirects_propertyUpdateDate",
                view: `/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/Timestamp.html?v=${cacheBuster}`,
                value: $scope.model.redirect ? $scope.model.redirect.updateDate : null,
                hello: moment(new Date($scope.model.redirect.updateDate)).fromNow(),
                readonly: true
            }
        ];
    };

    const allProperties = $scope.model.properties.concat($scope.model.advancedProperties, $scope.model.hiddenProperties);

    allProperties.concat($scope.model.infoProperties).forEach(function (p) {

        // Localize the label
        if (p.labelKey) {
            localizationService.localize(p.labelKey).then(function (value) {
                if (!value.length || value[0] === "[") return;
                p.label = value;
            });
        }
        
        // Localize the description
        if (p.descriptionKey) {
            localizationService.localize(p.descriptionKey).then(function (value) {
                if (!value.length || value[0] === "[") return;
                p.description = value;
            });
        }
        
        // Localize any config options
        if (p.config && p.config.options) {
            p.config.options.forEach(function (o) {
                if (!o.labelKey) return;
                localizationService.localize(o.labelKey).then(function (value) {
                    if (!value.length || value[0] === "[") return;
                    o.label = value;
                });
            });
        }

    });

    vm.settingsApp = {
        alias: "settings",
        name: "Settings",
        icon: "icon-equalizer",
        view: "nope",
        active: true
    };

    vm.infoApp = {
        alias: "info",
        name: "Info",
        view: "nope",
        icon: "icon-info"
    };

    $scope.model.navigation = $scope.model.redirect && $scope.model.redirect.id ? [vm.settingsApp, vm.infoApp] : [];

    function initLabels() {

        vm.labels = {
            addSuccessfulTitle: "Redirect added",
            addSuccessfulMessage: "Your redirect has successfully been added.",
            addFailedTitle: "Saving failed",
            addFailedMessage: "The redirect could not be added due to an error on the server.",
            saveSuccessfulTitle: "Redirect added",
            saveSuccessfulMessage: "Your redirect has successfully been added.",
            saveFailedTitle: "Saving failed",
            saveFailedMessage: "The redirect could not be saved due to an error on the server."
        };

        angular.forEach(vm.labels, function (_, key) {
            localizationService.localize(`redirects_${key}`).then(function (value) {
                if (!value.length || value[0] === "[") return;
                vm.labels[key] = value;
            });
        });

        localizationService.localize("redirects_settingsApp").then(function (value) {
            if (!value.length || value[0] === "[") return;
            vm.settingsApp.name = value;
        });
        
        localizationService.localize("redirects_infoApp").then(function (value) {
            if (!value.length || value[0] === "[") return;
            vm.infoApp.name = value;
        });

    }

    initLabels();

    vm.save = function () {

        // Map the properties back to an object we can send to the API
        const redirect = skybrudRedirectsService.propertiesToObject(allProperties);

        // Attempt to submit the form (Angular validation will kick in)
        if (!formHelper.submitForm({ scope: $scope })) return;

        // Reset the Angular form
        formHelper.resetForm({ scope: $scope });

        // Make sure we set a loading state
        vm.loading = true;

        // Make sure we set the "rootNodeKey" property as well
        if (redirect.rootNodeId > 0) {
            const rootNode = vm.options.rootNodes.find(x => x.id === redirect.rootNodeId);
            redirect.rootNodeKey = rootNode ? rootNode.key : "00000000-0000-0000-0000-000000000000";
        } else {
            redirect.rootNodeKey = "00000000-0000-0000-0000-000000000000";
        }

        if (redirect.key) {
            $http({
                method: "POST",
                url: `${baseUrl}EditRedirect`,
                params: {
                    redirectId: redirect.key
                },
                data: redirect
            }).then(function (r) {
                vm.loading = false;
                notificationsService.success(vm.labels.saveSuccessfulTitle, vm.labels.saveSuccessfulMessage);
                $scope.model.submit(r);
            }, function (res) {
                vm.loading = false;
                notificationsService.error(vm.labels.saveFailedTitle, res && res.data && res.data.meta ? res.data.meta.error : vm.labels.saveFailedMessage);
            });
        } else {
            $http({
                method: "POST",
                url: `${baseUrl}AddRedirect`,
                data: redirect
            }).then(function (r) {
                vm.loading = false;
                notificationsService.success(vm.labels.addSuccessfulTitle, vm.labels.addSuccessfulMessage);
                $scope.model.submit(r);
            }, function (res) {
                vm.loading = false;
                notificationsService.error(vm.labels.addFailedTitle, res && res.data && res.data.meta ? res.data.meta.error : vm.labels.addFailedMessage);
            });
        }

    };

    vm.close = function () {
        if ($scope.model.close) {
            $scope.model.close();
        } else {
            editorService.close();
        }
    };

});