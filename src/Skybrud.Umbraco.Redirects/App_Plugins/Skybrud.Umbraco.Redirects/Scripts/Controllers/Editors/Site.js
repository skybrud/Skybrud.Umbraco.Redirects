angular.module("umbraco").controller("SkybrudUmbracoRedirects.Editors.Site.Controller", function ($scope, localizationService, skybrudRedirectsService, editorState) {

    $scope.current = editorState.getCurrent();

    $scope.rootNodes = [
        { id: 0, name: "All sites" }
    ];

    $scope.loading = true;

    localizationService.localize("redirects_allSites").then(function (value) {
        $scope.rootNodes[0].name = value;
    });

    skybrudRedirectsService.getRootNodes().then(function (r) {
        r.data.items.forEach(function (rootNode) {
            $scope.rootNodes.push(rootNode);
            if ($scope.current && (`,${$scope.current.path},`).indexOf(`,${rootNode.id},`) > 0) {
                $scope.model.value = rootNode.id;
            }
        });
        $scope.loading = false;
    });

});