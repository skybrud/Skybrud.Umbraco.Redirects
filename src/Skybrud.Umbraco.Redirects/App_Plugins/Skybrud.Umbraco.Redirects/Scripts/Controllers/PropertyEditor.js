angular.module('umbraco').controller('SkybrudUmbracoRedirects.PropertyEditor.Controller', function ($scope, $routeParams, $http, $q, $timeout, dialogService, notificationsService, skybrudRedirectsService) {

    $scope.route = $routeParams;
    $scope.redirects = [];

    $scope.mode = $routeParams.create ? 'create' : 'list';

    $scope.loading = false;

    // If we're not in the content section, we stop further execution (eg. property editor preview)
    if ($scope.route.section != 'content') return;

    $scope.addRedirect = function () {
        var page = $scope.$parent.$parent.$parent.content;
        skybrudRedirectsService.addRedirect({
            page: page,
            callback: function () {
                $scope.updateList();
            }
        });
    };

    $scope.editRedirect = function (redirect) {
        skybrudRedirectsService.editRedirect(redirect, function () {
            $scope.updateList();
        });
    };

    $scope.deleteRedirect = function (redirect) {
        var url = redirect.url + (redirect.queryString ? '?' + redirect.queryString : '');
        if (!confirm('Are you sure you want do delete the redirect at "' + url + '" ?')) return;
        skybrudRedirectsService.deleteRedirect(redirect, function () {
            $scope.updateList();
        });
    };

    $scope.updateList = function () {

        $scope.loading = true;

        // Make the call to the redirects API
        var http = $http({
            method: 'GET',
            url: '/umbraco/backoffice/api/Redirects/GetRedirectsForContent',
            params: {
                contentId: $routeParams.id
            }
        });

        // Show the loader for at least 200 ms
        var timer = $timeout(function () { }, 200);

        // Wait for both the AJAX call and the timeout
        $q.all([http, timer]).then(function (array) {
            $scope.content = array[0].data.data.content;
            $scope.redirects = array[0].data.data.redirects;
            $scope.loading = false;
        }, function () {
            notificationsService.error('Unable to load redirects', 'The list of redirects could not be loaded.');
            $scope.loading = false;
        });

    };

    if ($scope.mode == 'list') {
        $scope.updateList();
    }

});