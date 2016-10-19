angular.module('umbraco').controller('SkybrudUmbracoRedirects.AddRedirectDialog.Controller', function ($scope, $http, skybrudLinkPickerService, notificationsService) {

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

    $scope.save = function () {

        if ($scope.loading) return;

        if (!$scope.redirect.url) {
            notificationsService.error('Empty field', 'You must specify an URL the redirect should match.');
            return;
        }

        if (!$scope.redirect.link) {
            notificationsService.error('Empty field', 'You must specify a destination link.');
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
            notificationsService.success('Redirect added', 'Your redirect was successfully added.');
            $scope.submit($scope.redirect);
        }).error(function (res) {
            $scope.loading = false;
            notificationsService.error('Adding redirect failed', res && res.meta ? res.meta.error : 'The server was unable to add your redirect.');
        });

    };

});