angular.module("umbraco").directive("tabFocus", [
    "$parse",
    "tabFocusService",
    function($parse, service) {
        return {
            restrict: "A",
            link: link
        };

        function link(scope, $element, attrs) {
            var tabFocusFn = $parse(attrs.tabFocus);

            function tabFocus() {
                tabFocusFn(scope);
            }

            var unsubscribe = service.subscribe($element, tabFocus);
            scope.$on("$destroy", unsubscribe);
        }
    }
]);
