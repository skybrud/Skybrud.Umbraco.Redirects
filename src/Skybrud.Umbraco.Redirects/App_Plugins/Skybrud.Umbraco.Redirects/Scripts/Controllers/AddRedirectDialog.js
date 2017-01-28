angular.module('umbraco').controller('SkybrudUmbracoRedirects.AddRedirectDialog.Controller', function ($scope, $http, notificationsService, skybrudRedirectsService) {

    $scope.options = $scope.dialogOptions.options;

    $scope.type = $scope.options && $scope.options.media ? 'media' : 'content';
    $scope.content = $scope.options && $scope.options.content;
    $scope.media = $scope.options && $scope.options.media;

    $scope.redirect = {
        url: '',
        link: null
    };


    if ($scope.content) {
        $scope.redirect.link = {
            id: $scope.content.id,
            name: $scope.content.name,
            url: $scope.content.urls.length > 0 ? $scope.content.urls[0] : '#',
            mode: 'content'
        }
    } else if ($scope.media) {

        // $scope.media doesn't expode the URL directly, so we need to read it from the "_umb_urls" property
        var mediaUrl = null;
        angular.forEach($scope.media.tabs, function (tab) {
            angular.forEach(tab.properties, function (property) {
                if (property.alias == '_umb_urls') {
                    mediaUrl = property.value;
                }
            });
        });

        $scope.redirect.link = {
            id: $scope.media.id,
            name: $scope.media.name,
            url: mediaUrl ? mediaUrl : '#',
            mode: 'media'
        }

    }

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
        return skybrudRedirectsService.isValidUrl($scope.redirect.url);
    };

    $scope.save = function () {

        if ($scope.loading) return;

        if (!$scope.redirect.url) {
            notificationsService.error('Ingen URL', 'Du skal angive den oprindelige URL.');
            return;
        }

        if (!skybrudRedirectsService.isValidUrl($scope.redirect.url)) {
            notificationsService.error('Ugyldig værdi', 'Den angivne URL er ikke gyldig.');
            return;
        }

        if (!$scope.redirect.link) {
            notificationsService.error('Intet link', 'Du skal angive en destinationsside eller -link.');
            return;
        }

        var params = {
            url: $scope.redirect.url,
            linkMode: $scope.type,
            linkId: $scope.redirect.link.id,
            linkUrl: $scope.redirect.link.url,
            linkName: $scope.redirect.link.name
        };

        $scope.loading = true;

        $http({
            method: 'GET',
            url: '/umbraco/backoffice/api/Redirects/AddRedirect',
            params: params
        }).success(function () {
            $scope.loading = false;
            notificationsService.success('Redirect oprettet', 'Dit redirect er nu blevet oprettet.');
            $scope.submit($scope.redirect);
        }).error(function (res) {
            $scope.loading = false;
            notificationsService.error('Oprettelse fejlede', res && res.meta ? res.meta.error : 'Grundet en fejl på serveren kunne dit redirect ikke oprettes.');
        });

    };

});