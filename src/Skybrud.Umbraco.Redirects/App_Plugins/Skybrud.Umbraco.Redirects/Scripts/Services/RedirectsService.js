angular.module('umbraco.services').factory('skybrudRedirectsService', function ($http, dialogService, notificationsService) {

    var service = {
        
        addRedirect: function (options) {

            if (!options) options = {};
            if (typeof (options) == 'function') options = { callback: options };

            var d = dialogService.open({
                template: '/App_Plugins/Skybrud.Umbraco.Redirects/Views/Dialogs/Add.html',
                show: true,
                options: options,
                callback: function (value) {
                    if (options.callback) options.callback(value);
                }
            });

            // Make the dialog 20px wider than default so it can be seen bhind the linkpicker dialog
            d.element[0].style = 'display: flex; width: 460px !important; margin-left: -460px';

        },
        
        editRedirect: function (redirect, options) {

            if (!options) options = {};
            if (typeof (options) == 'function') options = { callback: options };
            
            var d = dialogService.open({
                template: '/App_Plugins/Skybrud.Umbraco.Redirects/Views/Dialogs/Edit.html',
                show: true,
                redirect: redirect,
                options: options,
                callback: function (value) {
                    if (options.callback) options.callback(value);
                }
            });

            // Make the dialog 20px wider than default so it can be seen bhind the linkpicker dialog
            d.element[0].style = 'display: flex; width: 460px !important; margin-left: -460px';

        },

        deleteRedirect: function (redirect, callback) {
            $http({
                method: 'GET',
                url: '/umbraco/backoffice/api/Redirects/DeleteRedirect',
                params: {
                    redirectId: redirect.uniqueId
                }
            }).success(function () {
                notificationsService.success('Redirect deleted', 'Your redirect was successfully deleted.');
                if (callback) callback(redirect);
            }).error(function (res) {
                notificationsService.error('Deleting redirect failed', res && res.meta ? res.meta.error : 'The server was unable to delete your redirect.');
            });
        },

        isValidUrl: function(url) {

            // Make sure we have a string and trim all leading and trailing whitespace
            url = $.trim(url + '');

            // For now a valid URL should start with a forward slash
            return url.indexOf('/') === 0;

        }

    };

    return service;

});