angular.module("umbraco").controller("SkybrudUmbracoRedirects.Editors.Destination.Controller", function ($scope, editorService, eventsService, skybrudRedirectsService) {

    const vm = this;

    function editLink(link) {
        skybrudRedirectsService.editLink(link, function(model) {
            $scope.model.value = model;
            eventsService.emit("skybrud.umbraco.redirects.destination.updated", { destination: model });
            editorService.close();
        });
    }

    vm.addLink = function () {
        editLink();
    };

    vm.editLink = function () {
        editLink($scope.model.value);
    };

    vm.removeLink = function () {
        $scope.model.value = null;
        eventsService.emit("skybrud.umbraco.redirects.destination.updated", { destination: null });
    };

});