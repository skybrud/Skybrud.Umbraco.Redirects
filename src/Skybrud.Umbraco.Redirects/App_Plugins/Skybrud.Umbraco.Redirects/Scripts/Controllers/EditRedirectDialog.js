angular.module('umbraco').controller('SkybrudUmbracoRedirects.EditRedirectDialog.Controller', function ($scope, $http, notificationsService, skybrudLinkPickerService) {

    $scope.loading = false;

    // Make a copy of the redirect so we don't modify the object used in the list
    $scope.redirect = angular.copy($scope.dialogOptions.redirect);

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
            redirectId: $scope.redirect.uniqueId,
            url: $scope.redirect.url,
            linkMode: 'content',
            linkId: $scope.redirect.link.id,
            linkUrl: $scope.redirect.link.url,
            linkName: $scope.redirect.link.name
        };

        $scope.loading = true;

        $http({
            method: 'GET',
            url: '/umbraco/backoffice/api/Redirects/EditRedirect',
            params: params
        }).success(function () {
            $scope.loading = false;
            notificationsService.success('Redirect saved', 'Your redirect was successfully saved.');
            $scope.submit($scope.redirect);
        }).error(function (res) {
            $scope.loading = false;
            notificationsService.error('Saving redirect failed', res && res.meta ? res.meta.error : 'The server was unable to save your redirect.');
        });

    };

});