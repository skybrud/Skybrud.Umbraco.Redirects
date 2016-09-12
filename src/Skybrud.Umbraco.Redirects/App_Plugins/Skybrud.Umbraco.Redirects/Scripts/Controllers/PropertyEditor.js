angular.module('umbraco').controller('SkybrudUmbracoRedirects.PropertyEditor.Controller', function ($scope, $routeParams, $http, $q, $timeout, dialogService, skybrudLinkPickerService, notificationsService) {

    var lps = skybrudLinkPickerService;

    $scope.route = $routeParams;

    $scope.content = null;
    $scope.redirects = [];
    $scope.domains = [];

    $scope.mode = $routeParams.create ? 'create' : 'list';
    $scope.details = null;

    $scope.loading = {
        list: false,
        domains: false
    };

    $scope.loadRedirects = function () {

        $scope.loading.list = true;

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
            $scope.loading.list = false;
        }, function () {
            notificationsService.error('Unable to load redirects', 'The list of redirects could not be loaded.');
            $scope.loading.list = false;
        });

    };

    if ($scope.mode == 'list') {
        $scope.loadRedirects();
    }

});