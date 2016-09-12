angular.module("umbraco").controller("Skybrud.LinkPickerGridEditor.Controller", function ($scope) {

    // Hack to prevent Umbraco from stripping the configuration during save (see http://issues.umbraco.org/issue/U4-8743)
    $scope.config = $scope.control.editor.config;

});