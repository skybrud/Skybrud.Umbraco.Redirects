angular.module("umbraco").controller("SkybrudUmbracoRedirects.OutboundRedirectEditor.Controller", function ($scope, editorService, skybrudRedirectsService) {

    const vm = this;

    function editLink(link) {
        skybrudRedirectsService.editLink(link, function (model) {
            $scope.model.value = {
                permanent: vm.redirectType.value === "true",
                forward: vm.forward.value === "true",
                destination: model
            };
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
        $scope.model.value = "";
    };

    vm.updated = function () {
        if (!$scope.model.value) return;
        $scope.model.value.permanent = vm.redirectType.value === "true";
        $scope.model.value.forward = vm.forward.value === "true";
    };

    vm.redirectType = {
        uniqueId: `_${Math.random().toString(36).substr(2, 12)}`,
        value: $scope.model.value?.permanent === true ? "true" : "false",
        options: [
            {
                label: "Permanent",
                labelKey: "redirects_labelPermanent",
                value: "true"
            },
            {
                label: "Temporary",
                labelKey: "redirects_labelTemporary",
                value: "false"
            }
        ]
    };

    vm.forward = {
        uniqueId: `_${Math.random().toString(36).substr(2, 12)}`,
        value: $scope.model.value?.forward === true ? "true" : "false",
        options: [
            {
                label: "Enabled",
                labelKey: "redirects_labelEnabled",
                value: "true"
            },
            {
                label: "Disabled",
                labelKey: "redirects_labelDisabled",
                value: "false"
            }
        ]
    };

    if ($scope.model.value && !$scope.model.value.destination) $scope.model.value = "";

});