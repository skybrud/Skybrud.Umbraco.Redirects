angular.module('umbraco').controller('SkybrudUmbracoRedirects.AddRedirectDialog.Controller', function ($scope, $http, notificationsService, skybrudLinkPickerService, skybrudRedirectsService) {

    $scope.options = $scope.dialogOptions.options;
    $scope.page = $scope.options && $scope.options.page;

    $scope.redirect = {
        url: '',
        link: null
    };

    if ($scope.page) {
        $scope.redirect.link = {
            id: $scope.page.id,
            name: $scope.page.name,
            url: $scope.page.urls.length > 0 ? $scope.page.urls[0] : '#',
            mode: 'content'
        }
    }

    $scope.addLink = function () {
        skybrudLinkPickerService.addLink(function (link) {
            $scope.redirect.link = link;
        }, false);
    };

    $scope.editLink = function () {
        skybrudLinkPickerService.editLink($scope.redirect.link, function (link) {
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
            linkMode: 'content',
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