angular.module("perplexContentBlocks").component("contentBlocksPortal", {
    bindings: {
        to: "@",
        prepend: "<?"
    },
    transclude: true,
    template: "<ng-transclude></ng-transclude>",
    controller: [
        "$element",
        function contentBlocksPortalController($element) {
            this.$onInit = function() {
                var target = document.querySelector(this.to);

                if (target == null) {
                    throw new Error("content-blocks-portal: No DOM element found for selector '" + this.to + "'");
                }

                if (this.prepend) {
                    $element.prependTo(target);
                } else {
                    $element.appendTo(target);
                }
            }

            this.$onDestroy = function() {
                // Since we have moved the element manually
                // we also should clean it up ourselves.
                $element.remove();
            }
        }
    ],
});
