angular.module("umbraco").controller("SkybrudUmbracoRedirects.Editors.Destination.Controller", function ($scope, editorService, skybrudRedirectsService) {

    const vm = this;

    function editLink(link) {
        skybrudRedirectsService.editLink(link, function(model) {
            $scope.model.value = model;
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
    };

});