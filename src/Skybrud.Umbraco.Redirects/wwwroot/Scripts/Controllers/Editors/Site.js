angular.module("umbraco").controller("SkybrudUmbracoRedirects.Editors.Site.Controller", function ($scope, localizationService, skybrudRedirectsService, editorState) {

    const vm = this;

    vm.current = editorState.getCurrent();

    vm.rootNodes = [
        { id: 0, key: "", name: "All sites" }
    ];

    vm.loading = true;

    localizationService.localize("redirects_allSites").then(function (value) {
        vm.rootNodes[0].name = value;
    });

    skybrudRedirectsService.getRootNodes().then(function (r) {
        r.data.items.forEach(function (rootNode) {
            vm.rootNodes.push(rootNode);
            if (vm.current && (`,${vm.current.path},`).indexOf(`,${rootNode.id},`) > 0) {
                $scope.model.value = rootNode.id;
            }
        });
        vm.loading = false;
    });

});