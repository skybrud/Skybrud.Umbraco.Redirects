angular.module("umbraco").controller("SkybrudUmbracoRedirects.Editors.Destination.Controller", function ($scope, editorService, skybrudRedirectsService) {

    function editLink(link) {
        skybrudRedirectsService.editLink(link, function(model) {
            $scope.model.value = model;
            editorService.close();
        });
    }

    $scope.addLink = function () {
        editLink();
    };

    $scope.editLink = function () {
        editLink($scope.model.value);
    };

    $scope.removeLink = function () {
        $scope.model.value = null;
    };

});