angular.module("umbraco").controller("SkybrudUmbracoRedirects.Culture.Controller", function ($scope) {

    const vm = this;

    vm.updated = function () {
        $scope.model.value = $scope.model.culture.alias
    };

});