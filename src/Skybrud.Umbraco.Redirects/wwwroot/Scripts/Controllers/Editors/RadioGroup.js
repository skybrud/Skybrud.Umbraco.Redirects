angular.module("umbraco").controller("SkybrudUmbracoRedirects.Editors.RadioGroup.Controller", function () {
    const vm = this;
    vm.uniqueId = `_${Math.random().toString(36).substr(2, 12)}`;
});