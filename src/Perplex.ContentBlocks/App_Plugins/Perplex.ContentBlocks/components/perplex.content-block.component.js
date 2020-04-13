angular.module("umbraco").component("perplexContentBlock", {
    templateUrl: "/App_Plugins/Perplex.ContentBlocks/components/perplex.content-block.component.html",

    bindings: {
        block: "=",
        definition: "=",
        layouts: "=",
        categories: "=",
        isExpanded: "=",
        isMandatory: "=",
        preview: "=",
        name: "=",
        showSettings: "=",
        lazyLoad: "&?",
        canPaste: "=?",
        init: "&?",
        addBlock: "&?",
        paste: "&?",
        getValue: "&",
        setValue: "&",
        toggleExpand: "&",
        removeBlock: "&?",
        copyBlock: "&?",
        toggleDisableBlock: "&?",
        setLayout: "&",
        getLayoutIndex: "&",
        toggleSettings: "&",
        registerElement: "&?",
        isReorder: "=?",
        showAddContentButton: "<?",
    },

    controller: [
        "$element",
        perplexContentBlockController
    ]
});

function perplexContentBlockController($element) {
    var destroyFns = [];

    this.$onInit = function () {
        if (typeof this.init === "function") {
            this.init();
        }

        if (typeof this.registerElement === "function") {
            var removeFn = this.registerElement({ element: $element });
            if (typeof removeFn === "function") {
                destroyFns.push(removeFn);
            }
        }
    }

    this.$onDestroy = function () {
        destroyFns.forEach(function (destroyFn) {
            destroyFn();
        });
    }
}
