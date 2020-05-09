angular.module("perplexContentBlocks").directive("contentBlocksTabFocusOnce", [
    "$parse",
    "contentBlocksTabFocusService",
    tabFocusOnceDirective
]);

function tabFocusOnceDirective($parse, service) {
    return {
        restrict: "A",
        link: link
    };

    function link(scope, element, attrs) {
        var tabFocusOnceFn = $parse(attrs.contentBlocksTabFocusOnce);

        function tabFocusOnce(unsubscribeFn) {
            tabFocusOnceFn(scope);

            (unsubscribeFn || unsubscribe)();
        }

        var unsubscribe = service.subscribe(element, tabFocusOnce);
        scope.$on("$destroy", unsubscribe);
    }
}
