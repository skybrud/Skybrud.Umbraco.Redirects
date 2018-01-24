angular.module('umbraco').controller('SkybrudUmbracoRedirects.OutboundRedirectEditor.Controller', function ($scope) {

    if (!$scope.model.value) $scope.model.value = {};
    if ($scope.model.value.permanent === undefined) $scope.model.value.permanent = true;

    $scope.linkpicker = {items:[]};

    if ($scope.model.value.items) {
        $scope.linkpicker = $scope.model.value.items;
        delete $scope.model.value.items;
    } else if ($scope.model.value.link) {
        $scope.linkpicker = { items: [$scope.model.value.link] };
    }

    $scope.mode = $scope.model.value.permanent ? 'permanent' : 'temporary';

    $scope.config = {
        limit: 1
    };

    $scope.changed = function () {
        $scope.model.value.permanent = $scope.mode === 'permanent';
    };

    $scope.$watch('linkpicker', function () {
        $scope.model.value.link = $scope.linkpicker.items.length > 0 ? $scope.linkpicker.items[0] : {};
    }, true);

});