angular.module("umbraco").controller("Skybrud.Umbraco.Redirects.ContentApp.Controller", function ($scope, $routeParams, editorState) {

    // Declare the view model
    const vm = this;

    // Set information about the current node
    vm.node = {
        id: editorState.current.id,
        key: editorState.current.key,
        name: editorState.current.variants ? editorState.current.variants[0].name : editorState.current.name,
        url: null,
        published: false,
        trashed: editorState.current.trashed,
        type: editorState.current.udi.indexOf("umb://media/") === 0 ? "media" : "content",
        mode: $routeParams.create ? "create" : "list"
    };

    if (vm.node.type === "media") {
        vm.node.published = editorState.current.id > 0;
        vm.node.url = editorState.current.mediaLink;
    } else if (editorState.current.urls && editorState.current.urls.length > 0) {
        vm.node.published = editorState.current.urls[0].isUrl;
        vm.node.url = editorState.current.urls[0].text;
    }

});