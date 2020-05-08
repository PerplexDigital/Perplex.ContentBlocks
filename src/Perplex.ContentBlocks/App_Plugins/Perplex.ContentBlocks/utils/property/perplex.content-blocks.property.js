angular.module("perplexContentBlocks").component("contentBlocksProperty", {
    templateUrl: "/App_Plugins/Perplex.ContentBlocks/utils/property/perplex.content-blocks.property.html",
    bindings: {
        datatypeId: "<?",
        datatypeKey: "<?",
        config: "<?",
        getValue: "&?",
        setValue: "&?",
        onChange: "&?",
    },
    controller: [
        "contentBlocksPropertyScaffoldCache",
        contentBlocksPropertyController
    ]
});

function contentBlocksPropertyController(properyScaffoldCache) {
    var $ctrl = this;

    // The property that will be passed to <umb-property>
    // It is initialized in $onInit based on bindings.
    this.property = null;

    this.$onInit = function () {
        if (this.datatypeId == null && this.datatypeKey == null) {
            throw new Error("Pass in either a `datatypeId` or a `datatypeKey`");
        }

        properyScaffoldCache
            .getScaffold(this.datatypeId || this.datatypeKey)
            .then(this.applyScaffold.bind(this), this.handleError.bind(this));
    }

    this.handleError = function handleError(error) {
        if (error && (error.message || error.errorMsg)) {
            this.errorMsg = error.message || error.errorMsg;
        } else {
            this.errorMsg = "Error getting property type scaffold";
        }
    }

    this.applyScaffold = function applyScaffold(propertyTypeScaffold) {
        if (propertyTypeScaffold == null) {
            return;
        }

        this.property = propertyTypeScaffold;

        // For use in ContentBlocks the label should be hidden.    
        // If/when we add a custom label/description binding we could
        // use those to determine whether to hide the label or not.
        this.property.hideLabel = true;

        if (this.config != null && this.property.config != null) {
            Object.assign(this.property.config, this.config);
        }

        // The property's value will be stored externally
        // using get / set functions, if available

        var getValueIsFn = typeof this.getValue === "function";
        var setValueIsFn = typeof this.setValue === "function";
        var onChangeIsFn = typeof this.onChange === "function";

        if (getValueIsFn || setValueIsFn || onChangeIsFn) {
            var attributes = {};

            if (getValueIsFn) {
                attributes.get = function () {
                    return $ctrl.getValue();
                };
            }

            if (setValueIsFn || onChangeIsFn) {
                attributes.set = function (value) {
                    if (setValueIsFn) {
                        $ctrl.setValue({ value: value });
                    }

                    if (onChangeIsFn) {
                        $ctrl.onChange({ value: value });
                    }
                };
            }

            Object.defineProperty(this.property, "value", attributes);
        }
    }
}
