angular.module("umbraco").directive("tabFocusOnce", [
    "$parse",
    "tabFocusService",
    function($parse, service) {
        return {
            restrict: "A",
            link: link
        };

        function link(scope, $element, attrs) {
            var tabFocusOnceFn = $parse(attrs.tabFocusOnce);

            function tabFocusOnce(unsubscribeFn) {
                tabFocusOnceFn(scope);

                (unsubscribeFn || unsubscribe)();
            }

            var unsubscribe = service.subscribe($element, tabFocusOnce);
            scope.$on("$destroy", unsubscribe);
        }
    }
]);
