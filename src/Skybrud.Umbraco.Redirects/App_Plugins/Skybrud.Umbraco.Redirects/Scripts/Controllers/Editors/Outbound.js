angular.module("umbraco").controller("SkybrudUmbracoRedirects.OutboundRedirectEditor.Controller", function ($scope, editorService, skybrudRedirectsService) {

    const vm = this;

    if (!$scope.model.value) $scope.model.value = {};
    if ($scope.model.value.permanent === undefined) $scope.model.value.permanent = true;

    // Parse the redirect type
    vm.type = $scope.model.value.permanent ? "permanent" : "temporary";

    function editLink(link) {

        // Convert "link" to the target value the link picker can understand
        const target = skybrudRedirectsService.toUmbracoLink(link);

        // Open the link picker
        editorService.linkPicker({
            currentTarget: target,
            hideTarget: true,
            size: "medium",
            submit: function (m) {
                $scope.model.value.destination = skybrudRedirectsService.parseUmbracoLink(m.target);
                editorService.close();
            },
            close: function () {
                editorService.close();
            }
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