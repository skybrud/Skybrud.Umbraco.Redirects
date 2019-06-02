angular.module("umbraco").controller("SkybrudUmbracoRedirects.Editors.RadioGroup.Controller", function ($scope, editorService) {
	$scope.uniqueId = "_" + Math.random().toString(36).substr(2, 12);
});