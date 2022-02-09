angular.module("umbraco").controller("Skybrud.Umbraco.Redirects.InboundRedirects.Controller", function ($routeParams, editorState) {

    // Declare the view model
    const vm = this;

    // Set information about the current node
    vm.node = {
        id: editorState.current.id,
        key: editorState.current.key,
        name: editorState.current.variants ? editorState.current.variants[0].name : editorState.current.name,
        url: editorState.current.mediaLink ? editorState.current.mediaLink : editorState.current.urls[0].text,
        published: !!editorState.current.mediaLink || editorState.current.urls[0].isUrl,
        trashed: editorState.current.trashed,
        type: editorState.current.udi.indexOf("umb://media/") === 0 ? "media" : "content",
        mode: $routeParams.create ? "create" : "list"
    };

});