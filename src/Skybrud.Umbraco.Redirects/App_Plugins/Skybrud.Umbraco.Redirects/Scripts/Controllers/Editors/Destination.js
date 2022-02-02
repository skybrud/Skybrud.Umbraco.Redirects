angular.module("umbraco").controller("SkybrudUmbracoRedirects.Editors.Destination.Controller", function ($scope, editorService, skybrudRedirectsService) {

    function editLink(link) {

        if (link) {

            // Copy the link so we don't modify the input object
            link = angular.copy(link);

            if (!link.query) {
                const pos = link.url.indexOf("?");
                if (pos > 0) {
                    link.query = link.query.substr(pos + 1);
                    link.url = link.url.substr(0, pos);
                }
            }

            // Append the fragment to the URL (if specified)
            link.anchor = link.query + link.fragment;

        }

        // Open the link picker overlay
        editorService.linkPicker({
            currentTarget: link,
            hideTarget: true,
            submit: function (m) {
                $scope.model.value = skybrudRedirectsService.parseUmbracoLink(m.target);
                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        });

    }

    $scope.addLink = function () {
        editLink();
    };

    $scope.editLink = function () {
        editLink($scope.model.value);
    };

    $scope.removeLink = function () {
        $scope.model.value = null;
    };

});