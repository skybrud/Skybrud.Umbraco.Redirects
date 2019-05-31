angular.module('umbraco.services').factory('skybrudRedirectsService', function ($http, editorService, notificationsService) {

    var service = {

        parseUmbracoLink: function (e) {
            return {
                id: e.id || 0,
                name: e.name || '',
                url: e.url,
                target: e.target || '_self',
                isMedia: e.isMedia,
                udi: e.udi,
                mode: (e.id ? (e.isMedia || e.mode == 'media' ? 'media' : 'content') : 'url')
            };
        },

        addRedirect: function (options) {

            if (!options) options = {};
            if (typeof (options) == 'function') options = { callback: options };

            editorService.open({
                view: '/App_Plugins/Skybrud.Umbraco.Redirects/Views/Dialogs/Add.html',
                size: 'small',
                options,
                close: function () {
                    editorService.close();
                },
                submit: function (newData) {
                    editorService.close();

                    if ('callback' in options) options.callback();
                }
            });

        },
        
        editRedirect: function (redirect, options) {

            if (!options) options = {};
            if (typeof (options) == 'function') options = { callback: options };

            editorService.open({
                view: '/App_Plugins/Skybrud.Umbraco.Redirects/Views/Dialogs/Edit.html',
                size: 'small',
                redirect,
                options,
                close: function () {
                    editorService.close();
                },
                submit: function (newData) {
                    editorService.close();
                    if ('callback' in options) options.callback();
                }
            });

        },

        deleteRedirect: function (redirect, callback) {
            $http({
                method: 'GET',
                url: '/umbraco/backoffice/api/Redirects/DeleteRedirect',
                params: {
                    redirectId: redirect.uniqueId
                }
            }).then(function success() {
                notificationsService.success('Redirect deleted', 'Your redirect was successfully deleted.');
                if (callback) callback(redirect);
            }, function error(res) {
                notificationsService.error('Deleting redirect failed', res && res.meta ? res.meta.error : 'The server was unable to delete your redirect.');
            });
        },

        isValidUrl: function(url, isRegex) {

            // Make sure we have a string and trim all leading and trailing whitespace
            url = $.trim(url + '');

            // For now a valid URL should start with a forward slash
            return isRegex || url.indexOf('/') === 0;

        }

    };

    service.getRootNodes = function() {
        return $http.get('/umbraco/backoffice/api/Redirects/GetRootNodes');
    };

    return service;

});