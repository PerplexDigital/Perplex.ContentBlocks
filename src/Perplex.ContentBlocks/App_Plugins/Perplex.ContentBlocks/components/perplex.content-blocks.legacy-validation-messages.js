// Component to render validation messages in Umbraco < 8.7.
// In Umbraco 8.7 validation messages were revamped and show up 
// at each property including inside Nested Content, recursively. 
// This is a great improvement and we use that in ContentBlocks too.
// For older versions we render validation messages ourselves at each block with this component.
angular.module("perplexContentBlocks").component("perplexContentBlocksLegacyValidationMessages", {
    template: '<div ng-repeat="validationMessage in $ctrl.validationMessages track by validationMessage.property+validationMessage.errorMessage" style="display: flex;"><code ng-bind="validationMessage.property" ng-show="validationMessage.property" class="alert alert-info property-error"></code><div ng-bind="validationMessage.errorMessage" ng-show="validationMessage.errorMessage" class="alert alert-error property-error"></div></div>',
    bindings: { validationMessages: "<" },
});
