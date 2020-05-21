angular.module("perplexContentBlocks").component("contentBlocksIcon", {
    template:
        '<svg class="l-icon" ng-class="[\'l-icon--\' + $ctrl.size, $ctrl.cssClass]">' +
        '<use ng-attr-href="/App_Plugins/Perplex.ContentBlocks/assets/icons.svg#icon-{{$ctrl.icon}}"></use>' +
        '</svg> ',
    bindings: {
        icon: "@",
        size: "@?",
        cssClass: "@?",
    },
    controller: function () {
        this.$onInit = function () {
            if (this.size == null) {
                // Default to small
                this.size = "sm";
            }
        }
    }
});
