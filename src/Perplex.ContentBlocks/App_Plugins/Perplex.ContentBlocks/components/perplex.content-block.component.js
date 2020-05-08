angular.module("perplexContentBlocks").component("perplexContentBlock", {
    templateUrl: "/App_Plugins/Perplex.ContentBlocks/components/perplex.content-block.component.html",

    bindings: {
        block: "<",
        definition: "<",
        layouts: "<",
        categories: "<",
        isMandatory: "<",
        canPaste: "<?",
        init: "&?",
        addBlock: "&?",
        paste: "&?",
        getValue: "&",
        setValue: "&",
        removeBlock: "&?",
        copyBlock: "&?",
        setLayout: "&",
        getLayoutIndex: "&",
        registerElement: "&?",
        isReorder: "<?",
        showAddContentButton: "<?",
        registerCtrl: "&?",
        onOpen: "&?",
        onClose: "&?",
        disableAddContent: "<?",
        validationMessages: "<?",
        allowDisable: "<?",
    },

    controller: [
        "$element",
        "$interpolate",
        "contentBlocksPropertyScaffoldCache",
        perplexContentBlockController
    ]
});

function perplexContentBlockController($element, $interpolate, scaffoldCache) {
    var destroyFns = [];

    // State

    var state = this.state = {
        nameTemplate: "",

        open: false,
        load: false,
    };

    // Functions

    this.$onInit = function () {
        if (typeof this.init === "function") {
            this.init();
        }

        if (typeof this.registerElement === "function") {
            destroyFns.push(
                this.registerElement({ element: $element })
            );
        }

        if (typeof this.registerCtrl === "function") {
            destroyFns.push(
                this.registerCtrl({ ctrl: this })
            );
        }

        this.initName();
    }

    this.$onDestroy = function () {
        destroyFns.forEach(function (destroyFn) {
            if (typeof destroyFn === "function") {
                destroyFn();
            }
        });
    }

    this.initName = function () {
        if (this.definition == null) {
            return;
        }
        
        var scaffoldIdOrKey = this.definition.DataTypeId || this.definition.DataTypeKey;
        if (scaffoldIdOrKey != null) {
            scaffoldCache.getScaffold(scaffoldIdOrKey).then(function (scaffold) {
                if (scaffold != null) {
                    if (scaffold.editor !== "Umbraco.NestedContent") {
                        throw new Error("The data type editor should be \"Umbraco.NestedContent\", but is \"" + scaffold.editor + "\"");
                    }

                    state.nameTemplate = scaffold.config.contentTypes[0].nameTemplate;
                    this.updateName();
                }
            }.bind(this));
        }
    }

    this.updateName = function () {
        var content = this.block && this.block.content && this.block.content[0];
        this.name = $interpolate(state.nameTemplate)(content);
    }

    this.open = function () {
        state.load = true;

        var onOpen = typeof this.onOpen === "function"
            ? this.onOpen.bind(null, { block: this })
            : null;

        this.slideDown(onOpen);
    }

    this.close = function () {
        var onClose = typeof this.onClose === "function"
            ? this.onClose.bind(null, { block: this })
            : null;

        this.slideUp(onClose);
    }

    this.toggle = function () {
        if (this.state.open) {
            this.close();
        } else {
            this.open();
        }
    }

    this.slideUp = function (doneFn) {
        this.slide(true, doneFn);
    }

    this.slideDown = function (doneFn) {
        this.slide(false, doneFn);
    }

    this.slideToggle = function (doneFn) {
        this.slide(state.expand, doneFn);
    }

    this.slide = function (open, doneFn) {
        this.state.open = !open;

        var $main = $element.find(".p-block__main");
        if ($main.length === 0) {
            if (typeof doneFn === "function") {
                // No slide will happen, call doneFn now.
                doneFn();
            }
            return;
        }

        var slideFn = open ? $.fn.slideUp : $.fn.slideDown;
        slideFn.call($main, "fast", doneFn);
    }
}
