angular.module("umbraco").directive("tabRefocus", [
    "$parse",
    "tabFocusService",
    function($parse, service) {
        return {
            restrict: "A",
            link: link
        };

        function link(scope, $element, attrs) {
            var tabRefocusFn = $parse(attrs.tabRefocus);

            var skip = true;
            function tabRefocus() {
                if (skip) skip = false;
                else tabRefocusFn(scope);
            }

            var unsubscribe = service.subscribe($element, tabRefocus);
            scope.$on("$destroy", unsubscribe);
        }
    }
]);
