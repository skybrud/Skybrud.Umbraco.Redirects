angular.module("umbraco").controller("SkybrudUmbracoRedirects.Editors.OriginalUrl.Controller", function ($scope, $element, editorService) {

    $scope.isValidUrl = function (url, isRegex) {

		// Make sure we have a string and trim all leading and trailing whitespace
		url = $.trim(url + "");

		// For now a valid URL should start with a forward slash
		return isRegex || url.indexOf("/") === 0;

	};

});

angular.module("umbraco").directive("redirectsValidateUrl", function (skybrudRedirectsService) {
	return {
        require: "ngModel",
        link: function (scope, elem, attr, ngModel) {
			ngModel.$parsers.unshift(function (value) {
                ngModel.$setValidity("url", skybrudRedirectsService.isValidUrl(value));
                return value;
			});
		}
	};
});