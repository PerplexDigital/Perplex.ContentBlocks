angular.module("MyContentBlocksAddon").component("component", {
    templateUrl: "/App_Plugins/MyContentBlocksAddon/component.html",

    bindings: {
        block: "<",
        definition: "<",
    },

    require: {
        blockCtrl: "^perplexContentBlock"
    },

    controller: [
        function componentController() {
            this.$onInit = function () {
                // TODO
            }
        }
    ],
});
