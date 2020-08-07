angular.module("perplexContentBlocks").component("perplexContentBlock", {
    templateUrl: "/App_Plugins/Perplex.ContentBlocks/components/perplex.content-block.component.html",

    bindings: {
        block: "<",
        definition: "<",
        layouts: "<",
        categories: "<",
        isMandatory: "<",
        canPaste: "<?",
        onInit: "&?",
        addBlock: "&?",
        paste: "&?",
        getValue: "&",
        setValue: "&",
        removeBlock: "&?",
        copyBlock: "&?",
        registerElement: "&?",
        isReorder: "<?",
        showAddContentButton: "<?",
        registerCtrl: "&?",
        onOpen: "&?",
        onClose: "&?",
        disableAddContent: "<?",
        validationMessages: "<?",
        allowDisable: "<?",
        isLoading: "<",
        onOpenSettings: "&?",
    },

    controller: [
        "$element",
        "$interpolate",
        "contentBlocksPropertyScaffoldCache",
        "$scope",
        perplexContentBlockController
    ]
});

function perplexContentBlockController($element, $interpolate, scaffoldCache, $scope) {
    var destroyFns = [];

    // State

    var state = this.state = {
        nameTemplate: "",

        open: false,
        loadEditor: false,

        // Index of current layout
        layoutIndex: 0,
        sliderInitialized: false,
        initialized: false,
    };

    // Functions

    this.$onInit = function () {
        if (this.isLoading) {
            return;
        }

        this.init();
    }

    this.$onDestroy = function () {
        destroyFns.forEach(function (destroyFn) {
            if (typeof destroyFn === "function") {
                destroyFn();
            }
        });
    }

    this.$onChanges = function (changes) {
        if (changes.isLoading && changes.isLoading.previousValue && !changes.isLoading.currentValue) {
            // Changing from loading to not loading anymore: initialize.
            this.init();
        }

        if (changes.isReorder && !changes.isReorder.previousValue && changes.isReorder.currentValue) {
            // Editor data is saved and the editor is unloaded.
            // This is necessary to prevent a bug with RTE data disappearing
            // after reorder when the editor remains active.
            // Unloading first fixes this.
            this.unloadEditor();
        }
    }

    this.init = function () {
        if (state.initialized) {
            // Already initialized
            return;
        }

        if (typeof this.onInit === "function") {
            this.onInit();
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
        this.initLayoutIndex();

        state.initialized = true;
    }

    this.initName = function () {
        if (this.definition == null) {
            return;
        }

        var scaffoldIdOrKey = this.definition.DataTypeId == null ? this.definition.DataTypeKey : this.definition.DataTypeId;
        if (scaffoldIdOrKey != null) {
            scaffoldCache.getScaffold(scaffoldIdOrKey).then(function (scaffold) {
                if (scaffold != null) {
                    if (scaffold.editor !== "Umbraco.NestedContent") {
                        throw new Error("The data type editor should be \"Umbraco.NestedContent\", but is \"" + scaffold.editor + "\"");
                    }

                    if (scaffold.config != null && Array.isArray(scaffold.config.contentTypes) && scaffold.config.contentTypes.length > 0) {
                        state.nameTemplate = scaffold.config.contentTypes[0].nameTemplate;
                        this.updateName();
                    }
                }
            }.bind(this));
        }
    }

    this.updateName = function () {
        if (state.nameTemplate == null || state.nameTemplate.length === 0) {
            return;
        }

        var content = this.block && this.block.content && this.block.content[0];
        if (content != null) {
            this.name = $interpolate(state.nameTemplate)(content);
        }
    }

    this.open = function () {
        state.loadEditor = true;

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

    this.setLayout = function (index) {
        if (!Array.isArray(this.layouts) || this.layouts.length < index + 1) {
            return;
        }

        var layout = this.layouts[index];
        if (layout != null) {
            this.block.layoutId = layout.Id;
        }

        this.state.layoutIndex = index;
    }

    this.initLayoutIndex = function () {
        if (Array.isArray(this.layouts)) {
            this.state.layoutIndex = _.findIndex(this.layouts, function (layout) {
                return layout.Id === this.block.layoutId;
            }.bind(this));
        }
    }

    this.toggleSettings = function () {
        if (this.state.showSettings) {
            this.closeSettings();
        } else {
            this.openSettings();
        }
    }

    this.openSettings = function () {
        if (this.state.showSettings) return;

        this.state.showSettings = true;
        if (typeof this.onOpenSettings === "function") {
            this.onOpenSettings({ ctrl: this });
        }
    }

    this.closeSettings = function () {
        if (!this.state.showSettings) return;

        this.state.showSettings = false;
    }

    this.unloadEditor = function () {
        if (this.state.loadEditor) {
            // Signal Nested Content to save data -- calls our setValue().
            // If there is a better way than through this event: let us know.
            $scope.$broadcast("formSubmitting");

            // Unload
            this.state.loadEditor = false;
        }
    }
}
