angular.module("perplexContentBlocks").component("perplexContentBlocksLegacyValidationMessages", {
    template: '<div ng-repeat="validationMessage in $ctrl.validationMessages track by validationMessage.property+validationMessage.errorMessage" style="display: flex;"><code ng-bind="validationMessage.property" ng-show="validationMessage.property" class="alert alert-info property-error"></code><div ng-bind="validationMessage.errorMessage" ng-show="validationMessage.errorMessage" class="alert alert-error property-error"></div></div>',
    bindings: { validationMessages: "<" },
});
