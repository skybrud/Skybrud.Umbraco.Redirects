angular.module("umbraco").controller("SkybrudUmbracoRedirects.Culture.Controller", function (eventsService, $scope) {

    const vm = this;

    vm.updated = function () {
        $scope.model.value = $scope.model.culture.alias;
        eventsService.emit("skybrud.umbraco.redirects.culture.updated", {
            alias: $scope.model.culture.alias,
            culture: $scope.model.culture
        });
    };

});