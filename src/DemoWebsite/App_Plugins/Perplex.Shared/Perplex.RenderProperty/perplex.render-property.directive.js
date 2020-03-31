angular.module("umbraco").directive("perplexRenderProperty", [
    "perplexRenderPropertyService",
    function (renderPropertyService) {
        return {
            scope: {
                id: "=?",
                guid: "=?",
                alias: "=?",
                name: "=?",
                label: "=?",
                description: "=?",
                showLabel: "=?",
                config: "=?",
                getValue: "&?",
                setValue: "&?",
                onChange: "&?"
            },

            restrict: "E",
            replace: true,
            templateUrl: "/App_Plugins/Perplex.Shared/Perplex.RenderProperty/perplex.render-property.html",

            controller: function ($scope) {
                if ($scope.id == null && $scope.alias == null && $scope.guid == null) {
                    throw new Error("Pass in either an `id` or `alias` or `guid`");
                }

                var getValue = $scope.getValue;
                var setValue = $scope.setValue;
                var onChange = $scope.onChange;

                if ($scope.id != null) {
                    renderPropertyService.getPropertyTypeScaffoldById($scope.id).then(applyScaffold, handleError);
                } else if ($scope.guid != null){
                    renderPropertyService.getPropertyTypeScaffoldByGuid($scope.guid).then(applyScaffold, handleError);
                } else {
                    renderPropertyService.getPropertyTypeScaffoldByAlias($scope.alias, $scope.name).then(applyScaffold, handleError);
                }

                function handleError(error) {
                    if(error && (error.message || error.errorMsg)) {
                        $scope.errorMsg = error.message || error.errorMsg;
                    } else {
                        $scope.errorMsg = "Property could not be rendered";
                    }
                }

                function applyScaffold(propertyTypeScaffold) {
                    if(propertyTypeScaffold == null) {
                        return;
                    }

                    $scope.property = propertyTypeScaffold;
                    $scope.property.hideLabel = !$scope.label && !$scope.showLabel;
                    $scope.property.label = $scope.label;
                    $scope.property.description = $scope.description;

                    if ($scope.config != null && $scope.property.config != null) {
                        Object.assign($scope.property.config, $scope.config);
                    }

                    // The property's value will be stored externally
                    // using get / set functions, if available
                    var getValueIsFn = typeof getValue === "function";
                    var setValueIsFn = typeof setValue === "function";
                    var onChangeIsFn = typeof onChange === "function";

                    if (getValueIsFn || setValueIsFn || onChangeIsFn) {
                        var attributes = {};

                        if (getValueIsFn) {
                            attributes.get = function () {
                                return getValue();
                            };
                        }

                        if (setValueIsFn || onChangeIsFn) {
                            attributes.set = function (value) {
                                if (setValueIsFn) {
                                    setValue({ value: value });
                                }

                                if (onChangeIsFn) {
                                    onChange({ value: value });
                                }
                            };
                        }

                        Object.defineProperty($scope.property, "value", attributes);
                    }
                }
            },
        };
    }
]);
