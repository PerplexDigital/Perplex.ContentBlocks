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

    this.open = function () {
        state.load = true;
        this.slideDown();
        if (typeof this.onOpen === "function") {
            this.onOpen({ block: this });
        }
    }

    this.close = function () {
        this.slideUp();
        if (typeof this.onClose === "function") {
            this.onClose({ block: this });
        }
    }

    this.toggle = function () {
        if (this.state.open) {
            this.close();
        } else {
            this.open();
        }
    }

    this.slideUp = function () {
        this.slide(true);
    }

    this.slideDown = function () {
        this.slide(false);
    }

    this.slideToggle = function () {
        this.slide(state.expand);
    }

    this.slide = function (open) {
        this.state.open = !open;

        var $main = $element.find(".p-block__main");
        if ($main.length === 0) {
            return;
        }

        var slideFn = open ? $.fn.slideUp : $.fn.slideDown;
        slideFn.call($main, "fast");
    }
}
