angular.module("MyContentBlocksAddon").component("mcbaPersonalize", {
    templateUrl: "/App_Plugins/MyContentBlocksAddon/mcba-personalize.html",

    bindings: {
        block: "<",
        definition: "<",
    },

    require: {
        blockCtrl: "^perplexContentBlock"
    },

    controller: [
        function mcbaButton() {
            this.$onInit = function () {
                // TODO
            }

            this.personalize = function () {
                alert("TODO BRAM");
            }
        }
    ],
});
