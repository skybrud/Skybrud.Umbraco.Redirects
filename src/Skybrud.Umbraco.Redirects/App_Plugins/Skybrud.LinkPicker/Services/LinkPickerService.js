angular.module('umbraco.services').factory('skybrudLinkPickerService', function (dialogService) {

    var service = {

        parseUmbracoLink: function (e) {
            return {
                id: e.id || 0,
                name: e.name || '',
                url: e.url,
                target: e.target || '_self',
                mode: (e.id ? (e.isMedia ? 'media' : 'content') : 'url')
            };
        },

        addLink: function (callback, closeAllDialogs) {
            closeAllDialogs = closeAllDialogs !== false;
            if (closeAllDialogs) dialogService.closeAll();
            dialogService.linkPicker({
                callback: function (e) {
                    if (!e.id && !e.url && !confirm('The selected link appears to be empty. Do you want to continue anyways?')) return;
                    if (callback) callback(service.parseUmbracoLink(e));
                    if (closeAllDialogs) dialogService.closeAll();
                }
            });
        },

        editLink: function (link, callback, closeAllDialogs) {
            closeAllDialogs = closeAllDialogs !== false;
            if (closeAllDialogs) dialogService.closeAll();
            if (link.mode == 'media') {
                dialogService.linkPicker({
                    currentTarget: {
                        name: link.name,
                        url: link.url,
                        target: link.target
                    },
                    callback: function (e) {
                        if (!e.id && !e.url && !confirm('The selected link appears to be empty. Do you want to continue anyways?')) return;
                        if (service.parseUmbracoLink(e).id == 0) {
                            e.id = link.id;
                            e.isMedia = true;
                        }
                        if (callback) callback(service.parseUmbracoLink(e));
                        if (closeAllDialogs) dialogService.closeAll();
                    }
                });
            } else {
                dialogService.linkPicker({
                    currentTarget: {
                        id: link.id,
                        name: link.name,
                        url: link.url,
                        target: link.target
                    },
                    callback: function (e) {
                        if (!e.id && !e.url && !confirm('The selected link appears to be empty. Do you want to continue anyways?')) return;
                        if (callback) callback(service.parseUmbracoLink(e));
                        if (closeAllDialogs) dialogService.closeAll();
                    }
                });
            }
        }

    };

    return service;

});