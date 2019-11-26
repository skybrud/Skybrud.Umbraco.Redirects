angular.module("umbraco").controller("SkybrudUmbracoRedirects.Editors.Site.Controller", function ($scope) {
	
	$scope.sitepicker = {
		rootNodes: []
	};
	
	if ($scope.model.value.rootNodes) {
		for (var i = 0; i < $scope.model.value.rootNodes.length; i++) {
			var rootNode = $scope.model.value.rootNodes[i];

			(function (node) {
				if (typeof node.id === "undefined") {
					node.id = 0;
				}

				$scope.sitepicker.rootNodes.push(node);
			})(rootNode);
		}

		$scope.model.value = $scope.model.value.rootNodeId;
	}
});