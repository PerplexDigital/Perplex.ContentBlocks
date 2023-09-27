angular.module("perplexContentBlocks").component("perplexContentBlocksAddBlock", {
    templateUrl: "/App_Plugins/Perplex.ContentBlocks/components/perplex.content-blocks.add-block.html",

    bindings: {
        addBlock: "&",
        paste: "&",
        canPaste: "<",
        show: "<",
        isHeader: "<",
        noPaddingTop: "<",
    },
});
