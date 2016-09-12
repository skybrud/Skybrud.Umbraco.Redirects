angular.module("umbraco").controller("Skybrud.LinkPickerPreValues.Controller", function ($scope) {

    if (!$scope.model.value) {
        $scope.model.value = {
            limit: 0,
            types: {
                url: true,
                content: true,
                media: true
            },
            showTable: false,
            columns: {
                type: true,
                id: true,
                name: true,
                url: true,
                target: true
            }
        };
    }

    $scope.update = function () {

        $scope.model.value.limit = parseInt($scope.model.value.limit) | 0;

    };

});