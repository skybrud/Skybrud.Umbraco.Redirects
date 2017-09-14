angular.module('umbraco').controller('SkybrudUmbracoRedirects.EditRedirectDialog.Controller', function ($scope, $http, notificationsService, skybrudRedirectsService) {

    $scope.loading = false;

    $scope.options = $scope.dialogOptions.options || {};

    // Make a copy of the redirect so we don't modify the object used in the list
    $scope.redirect = angular.copy($scope.dialogOptions.redirect);

    // Combine the URL and query string so we can bind to it in a single input field
    $scope.redirectUrl = $scope.redirect.url + ($scope.redirect.queryString ? "?" + $scope.redirect.queryString : "");

    $scope.hideRootNodeOption = $scope.options.hideRootNodeOption === '1' || $scope.options.hideRootNodeOption === true;
    
    $scope.rootNodes = [
        { id: 0, name: 'All sites' }
    ];

    $scope.rootNode = $scope.rootNodes[0];

    $scope.rootNodeChanged = function () {
        $scope.redirect.rootNodeId = $scope.rootNode.id;
    };

    $scope.addLink = function () {
        skybrudRedirectsService.addLink(function (link) {
            $scope.redirect.link = link;
        }, false);
    };

    $scope.editLink = function () {
        skybrudRedirectsService.editLink($scope.redirect.link, function (link) {
            $scope.redirect.link = link;
        }, false);
    };

    $scope.hasValidUrl = function () {
        return skybrudRedirectsService.isValidUrl($scope.redirect.url, $scope.redirect.regex);
    };

    $scope.save = function () {

        if ($scope.loading) return;

        if (!$scope.redirectUrl) {
            notificationsService.error('Ingen URL', 'Du skal angive den oprindelige URL.');
            return;
        }

        if (!skybrudRedirectsService.isValidUrl($scope.redirect.url, $scope.redirect.regex)) {
            notificationsService.error('Ugyldig værdi', 'Den angivne URL er ikke gyldig.');
            return;
        }

        if (!$scope.redirect.link) {
            notificationsService.error('Intet link', 'Du skal angive en destinationsside eller -link.');
            return;
        }

        var params = {
            redirectId: $scope.redirect.uniqueId,
            rootNodeId: $scope.redirect.rootNodeId,
            url: $scope.redirectUrl,
            linkMode: $scope.redirect.link.mode,
            linkId: $scope.redirect.link.id,
            linkUrl: $scope.redirect.link.url,
            linkName: $scope.redirect.link.name,
            permanent: $scope.redirect.permanent,
            regex: $scope.redirect.regex,
            forward: $scope.redirect.forward
        };

        $scope.loading = true;

        $http({
            method: 'GET',
            url: '/umbraco/backoffice/api/Redirects/EditRedirect',
            params: params
        }).success(function () {
            $scope.loading = false;
            notificationsService.success('Redirect gemt', 'Dit redirect er nu blevet gemt.');
            $scope.submit($scope.redirect);
        }).error(function (res) {
            $scope.loading = false;
            notificationsService.error('Redigering fejlede', res && res.meta ? res.meta.error : 'Grundet en fejl på serveren kunne dit redirect ikke gemmes.');
        });

    };

    skybrudRedirectsService.getRootNodes().success(function (r) {
        angular.forEach(r.data, function (rootNode) {
            $scope.rootNodes.push(rootNode);

            // If a property editor for content, the current root node (if present) should be pre-selected 
            if ($scope.redirect.rootNodeId == rootNode.id) {
                $scope.rootNode = rootNode;
                $scope.rootNodeChanged();
            }

        });
    });

});