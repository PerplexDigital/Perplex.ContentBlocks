angular.module("MyContentBlocksAddon").component("mcbaButton", {
    templateUrl: "/App_Plugins/MyContentBlocksAddon/mcba-button.html",

    bindings: {
        block: "<",
        definition: "<",
    },

    require: {
        blockCtrl: "^perplexContentBlock"
    },
});
