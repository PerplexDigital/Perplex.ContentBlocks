angular.module("umbraco")
    .decorator("nestedContentPropertyEditorDirective", function ($delegate, versionHelper) {
        // All versions below 8.15.0 need this patch.
        var patchIsNeeded = versionHelper.versionCompare(Umbraco.Sys.ServerVariables.application.version, "8.15.0") < 0;

        if (patchIsNeeded && Array.isArray($delegate) && $delegate.length > 0) {
            var component = $delegate[0];
            if (typeof component.template === "string") {
                component.template += "<content-blocks-nested-content-patch></content-blocks-nested-content-patch>";
            }
        }

        return $delegate;
    });

angular.module("perplexContentBlocks").component("contentBlocksNestedContentPatch", {
    controller: [function contentBlocksNestedContentPatchController() {
        var $ctrl = this;

        this.$onInit = function() {
            if(this.ncEditor != null && Array.isArray(this.ncEditor.scaffolds)) {
                var pushFn = this.ncEditor.scaffolds.push;
                this.ncEditor.scaffolds.push = function() {
                    for(var i = 0; i < arguments.length; i++) {
                        var scaffold = arguments[i];
                        ensureCultureData(scaffold, $ctrl.ncEditor);
                    }

                    pushFn.apply($ctrl.ncEditor.scaffolds, arguments);
                }
            }
        }

        // Code from PR 10562:
        // https://github.com/umbraco/Umbraco-CMS/pull/10562/files
        // Updated arrow functions to regular functions and passed in "vm" variable
        // since we are running in a different context.
        function ensureCultureData(content, vm) {
            if (!content || !vm.umbVariantContent || !vm.umbProperty) return;

            if (vm.umbVariantContent.editor.content.language) {
                // set the scaffolded content's language to the language of the current editor
                content.language = vm.umbVariantContent.editor.content.language;
            }
            // currently we only ever deal with invariant content for blocks so there's only one
            content.variants[0].tabs.forEach(function(tab) {
                tab.properties.forEach(function(prop) {
                    // set the scaffolded property to the culture of the containing property
                    prop.culture = vm.umbProperty.property.culture;
                });
            });
        }
    }],

    require: {
        ncEditor: "^nestedContentPropertyEditor",
    }
});
