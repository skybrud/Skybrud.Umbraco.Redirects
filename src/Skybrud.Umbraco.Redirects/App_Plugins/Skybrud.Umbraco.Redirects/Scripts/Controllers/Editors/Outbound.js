angular.module("umbraco").controller("SkybrudUmbracoRedirects.OutboundRedirectEditor.Controller", function ($scope, editorService, skybrudRedirectsService) {

    const vm = this;

    if (!$scope.model.value) $scope.model.value = {};
    if ($scope.model.value.permanent === undefined) $scope.model.value.permanent = true;

    // Parse the redirect type
    vm.type = $scope.model.value.permanent ? "permanent" : "temporary";

    function editLink(link) {
        skybrudRedirectsService.editLink(link, function(model) {
            $scope.model.value.destination = model;
            editorService.close();
        });
    }

    vm.addLink = function () {
        editLink();
    };

    vm.editLink = function () {
        editLink($scope.model.value.destination);
    };

    vm.removeLink = function () {
        $scope.model.value.destination = null;
    };

    vm.changed = function() {
        $scope.model.value.permanent = vm.type === "permanent";
    };


});