angular.module("perplexContentBlocks").component("perplexContentBlock", {
    templateUrl: "/App_Plugins/Perplex.ContentBlocks/components/perplex.content-block.component.html",

    bindings: {
        block: "<",
        definition: "<",
        layouts: "<",
        categories: "<",
        isExpanded: "<",
        isMandatory: "<",
        showSettings: "<",
        lazyLoad: "&?",
        canPaste: "<?",
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
        isReorder: "<?",
        showAddContentButton: "<?",
    },

    controller: [
        "$element",
        "$interpolate",
        "perplexRenderPropertyService",
        perplexContentBlockController
    ]
});

function perplexContentBlockController($element, $interpolate, renderPropertyService) {
    var destroyFns = [];

    var state = {
        nameTemplate: "",
    };

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

        this.initName();
    }

    this.$onDestroy = function () {
        destroyFns.forEach(function (destroyFn) {
            destroyFn();
        });
    }

    this.initName = function () {
        var getScaffoldFn = null;

        if (this.definition.DataTypeId) {
            getScaffoldFn = renderPropertyService.getPropertyTypeScaffoldById(this.definition.DataTypeId);
        } else if (this.definition.DataTypeKey) {
            getScaffoldFn = renderPropertyService.getPropertyTypeScaffoldByGuid(this.definition.DataTypeKey);
        }

        if (getScaffoldFn) {
            getScaffoldFn.then(function (scaffold) {
                state.nameTemplate = scaffold.config.contentTypes[0].nameTemplate;
                this.updateName();
            }.bind(this));
        }
    }

    this.updateName = function () {
        var content = this.block && this.block.content && this.block.content[0];
        this.name = $interpolate(state.nameTemplate)(content);
    }
}
