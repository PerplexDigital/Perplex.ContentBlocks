angular.module("umbraco").directive("perplexContentBlock", [
    function () {
        return {
            restrict: "E",
            replace: true,
            scope: {
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
                isHeader: "=?",
                isReorder: "=?",
                isLastBlock: "=?",
                hasContent: "=?"
            },

            templateUrl: "/App_Plugins/Perplex.ContentBlocks/components/perplex.content-block.component.html",

            link: function ($scope, $element) {
                if (typeof $scope.init === "function") {
                    $scope.init();
                }

                if (typeof $scope.registerElement === "function") {
                    var removeFn = $scope.registerElement({ element: $element });
                    if (typeof removeFn === "function") {
                        $scope.$on("$destroy", function () {
                            removeFn();
                        })
                    }
                }
            }
        }
    }
]);