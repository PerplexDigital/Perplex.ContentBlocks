angular.module("MyContentBlocksAddon", ["perplexContentBlocks"])
angular.module("umbraco").requires.push("MyContentBlocksAddon");

angular.module("MyContentBlocksAddon").run([
    "perplexContentBlocksCustomComponents",
    function (customComponents) {
        customComponents.block.main = "mcba-main";
        customComponents.block.buttons.push("mcba-button", "mcba-personalize");
    }]);
