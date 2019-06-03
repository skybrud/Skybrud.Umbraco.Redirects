angular.module("umbraco").controller("SkybrudUmbracoRedirects.Editors.Destination.Controller", function ($scope, editorService, skybrudRedirectsService) {

    function editLink(link) {
        editorService.linkPicker({
            currentTarget: link,
            hideTarget: true,
            submit: function (m) {
                $scope.model.value = skybrudRedirectsService.parseUmbracoLink(m.target);
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
        editLink($scope.model.value);
    };

    $scope.removeLink = function () {
        $scope.model.value = null;
    };

});