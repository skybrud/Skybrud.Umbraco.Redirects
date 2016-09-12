angular.module('umbraco').controller('SkybrudUmbracoRedirects.Dashboard.Controller', function ($scope, $http, $q, $timeout, dialogService, skybrudLinkPickerService, notificationsService) {

    var lps = skybrudLinkPickerService;

    $scope.redirects = [];
    $scope.domains = [];

    $scope.mode = 'list';

    $scope.details = null;

    $scope.loading = {
        list: false,
        domains: false
    };

    $scope.add = function () {

        $scope.details = {
            domain: null,
            url: '',
            link: null
        };

        $scope.loadDomains(function () {
            $scope.mode = 'add';
        });

    };

    $scope.editRedirect = function (redirect) {

        $scope.details = {
            redirect: redirect,
            domain: null,
            url: redirect.url + (redirect.queryString ? '?' + redirect.queryString : ''),
            link: redirect.link
        };

        $scope.loadDomains(function () {
            $scope.mode = 'edit';
        });

    };

    $scope.deleteRedirect = function (redirect) {

        var url = redirect.url + (redirect.queryString ? '?' + redirect.queryString : '');

        if (!confirm('Are you sure you want do delete the redirect at "' + url + '" ?')) return;

        $http({
            method: 'GET',
            url: '/umbraco/backoffice/api/Redirects/DeleteRedirect',
            params: {
                redirectId: redirect.uniqueId
            }
        }).success(function () {
            notificationsService.success('Redirect deleted', 'Your redirect was successfully deleted.');
            $scope.mode = 'list';
            $scope.loadRedirects();
        }).error(function (res) {
            notificationsService.error('Deleting redirect failed', res && res.meta ? res.meta.error : 'The server was unable to delete your redirect.');
        });

    };

    $scope.addSubmit = function() {

        var domainId = $scope.details.domain ? $scope.details.domain.id : $scope.details.domain;

        if (!$scope.details.url) {
            notificationsService.error('Empty field', 'You must specify an URL the redirect should match.');
            return;
        }

        if (!$scope.details.link) {
            notificationsService.error('Empty field', 'You must specify a destination link.');
            return;
        }

        var params = {
            url: $scope.details.url,
            linkMode: $scope.details.link.mode,
            linkId: $scope.details.link.id,
            linkUrl: $scope.details.link.url,
            linkName: $scope.details.link.name
        };

        $http({
            method: 'GET',
            url: '/umbraco/backoffice/api/Redirects/AddRedirect',
            params: params
        }).success(function () {
            $scope.mode = 'list';
            $scope.loadRedirects();
            notificationsService.success('Redirect added', 'Your redirect was successfully added.');
        }).error(function (res) {
            notificationsService.error('Adding redirect failed', res && res.meta ? res.meta.error : 'The server was unable to add your redirect.');
        });

    };

    $scope.editRedirectSubmit = function () {

        if (!$scope.details.url) {
            notificationsService.error('Empty field', 'You must specify an URL the redirect should match.');
            return;
        }

        if (!$scope.details.link || !$scope.details.link.url) {
            notificationsService.error('Empty field', 'You must specify a destination link.');
            return;
        }

        var params = {
            redirectId: $scope.details.redirect.uniqueId,
            url: $scope.details.url,
            linkMode: $scope.details.link.mode,
            linkId: $scope.details.link.id,
            linkUrl: $scope.details.link.url,
            linkName: $scope.details.link.name
        };

        $http({
            method: 'GET',
            url: '/umbraco/backoffice/api/Redirects/EditRedirect',
            params: params
        }).success(function () {
            notificationsService.success('Redirect saved', 'Your redirect was successfully saved.');
            $scope.mode = 'list';
            $scope.loadRedirects();
        }).error(function (res) {
            notificationsService.error('Saving redirect failed', res && res.meta ? res.meta.error : 'The server was unable to save your redirect.');
        });

    };

    $scope.cancel = function() {
        $scope.mode = 'list';
    };

    $scope.loadDomains = function (callback) {

        $scope.loading.domains = true;

        $http.get('/umbraco/backoffice/api/Redirects/GetDomains').success(function(domains) {
            $scope.domains = domains.data;
            $scope.loading.domains = false;
            if (callback) callback(domains);
        });

    };

    $scope.loadRedirects = function () {

        $scope.loading.list = true;




        // Make the call to the redirects API
        var http = $http({
            method: 'GET',
            url: '/umbraco/backoffice/api/Redirects/GetRedirects'
        });

        // Show the loader for at least 200 ms
        var timer = $timeout(function () { }, 200);

        // Wait for both the AJAX call and the timeout
        $q.all([http, timer]).then(function (array) {
            $scope.redirects = array[0].data.data;
            $scope.loading.list = false;
        }, function () {
            notificationsService.error('Unable to load redirects', 'The list of redirects could not be loaded.');
            $scope.loading.list = false;
        });

    };

    $scope.addLink = function () {
        lps.addLink(function (link) {
            $scope.details.link = link;
        });
    };

    $scope.editLink = function () {
        lps.editLink($scope.details.link, function (link) {
            $scope.details.link = link;
        });
    };

    $scope.loadRedirects();

});