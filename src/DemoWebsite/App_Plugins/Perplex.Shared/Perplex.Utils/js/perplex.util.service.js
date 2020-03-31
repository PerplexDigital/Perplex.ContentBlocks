angular.module("umbraco").service("Perplex.Util.Service", ["editorState", function (editorState) {
    function watchPropWithAlias($scope, alias, editor, callback) {
        var parentScope = getContentTabs($scope);
        if (parentScope && parentScope.content && parentScope.content.tabs) {
            var tabs = parentScope.content.tabs;
            for (var tabi in tabs) {
                var tab = tabs[tabi];
                for (var propi in tab.properties) {
                    var prop = tab.properties[propi];
                    if (prop && prop.alias === alias) {
                        if (prop.editor === editor || editor === null) {
                            var debouncedCallback = _.debounce(function (a, b, c) {
                                $scope.$apply(callback(a, b, c));
                            }, 500);
                            debouncedCallback(prop.value);
                            return parentScope.$watch(
                                "content.tabs[" + tabi + "].properties[" + propi + "].value",
                                debouncedCallback,
                                true
                            );
                        } else {
                            console.error("Editor is no match, editor: ", prop.editor);
                        }
                    }
                }
            }
        }
    };

    function isNodeDirty() {
        var form = angular.element("form[name=contentForm]");
        return form.hasClass("ng-dirty");
    };

    function getContentTabs($scope) {
        var parent = $scope.$parent;
        if (!parent.content || !parent.content.tabs)
            parent = getContentTabs(parent);

        return parent;
    }

    function getCurrentCulture() {
        var es = editorState.current;

        if (es == null || !Array.isArray(es.variants)) {
            return null;
        }

        var activeVariant = _.find(es.variants, function (variant) {
            return variant.active;
        });

        if (activeVariant !== null && activeVariant.language != null) {
            return activeVariant.language.culture;
        }

        return null;
    }   

    // Public API
    this.getCurrentCulture = getCurrentCulture;
    this.watchPropWithAlias = watchPropWithAlias;
    this.isNodeDirty = isNodeDirty;
}]);
