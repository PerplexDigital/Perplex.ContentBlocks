angular.module("MyContentBlocksAddon").component("mcbaMain", {
    templateUrl: "/App_Plugins/MyContentBlocksAddon/mcba-main.html",

    bindings: {
        block: "<",
        definition: "<",
    },

    require: {
        blockCtrl: "^perplexContentBlock"
    },

    controller: [
        function mcbaMain() {
            this.$onInit = function () {
                // TODO
            }
        }
    ],
});
