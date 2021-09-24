angular.module("MyContentBlocksAddon", ["perplexContentBlocks"])
angular.module("umbraco").requires.push("MyContentBlocksAddon");

angular.module("MyContentBlocksAddon").run([
    "perplexContentBlocksCustomComponents",
    function (customComponents) {
        customComponents.block.main = "component";
    }]);
