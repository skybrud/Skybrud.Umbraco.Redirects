angular.module("umbraco").controller("SkybrudUmbracoRedirects.OutboundRedirectEditor.Controller", function ($scope, editorService, skybrudRedirectsService) {

    if (!$scope.model.value) $scope.model.value = {};
    if ($scope.model.value.permanent === undefined) $scope.model.value.permanent = true;

    
    $scope.mode = $scope.model.value.permanent ? "permanent" : "temporary";

    function editLink(link) {
        editorService.linkPicker({
            currentTarget: link,
            hideTarget: true,
            submit: function (m) {
                $scope.model.value.destination = skybrudRedirectsService.parseUmbracoLink(m.target);
                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        });
    }

    $scope.addLink = function () {
        editLink();
    };

    $scope.editLink = function () {
        editLink($scope.model.value.destination);
    };

    $scope.removeLink = function () {
        $scope.model.value.destination = null;
    };

    $scope.changed = function() {
        $scope.model.value.permanent = $scope.mode === "permanent";
    };


});