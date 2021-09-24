angular.module("perplexContentBlocks").component("perplexContentBlocksCustomComponent", {
    bindings: {
        component: "<",
        data: "<",
    },
    controller: [
        "$scope",
        "$compile",
        "$element",
        function perplexContentBlocksCustomComponent($scope, $compile, $element) {
            this.$onInit = function () {
                if (this.component == null) {
                    throw new Error("perplexContentBlocksCustomComponent: component binding is required but missing");
                }

                this.renderCustomComponent();
            }

            this.renderCustomComponent = function () {
                var params = this.data == null ? "" : " " + Object.keys(this.data).map(function (key) {
                    return key + '="$ctrl.data[\'' + key + '\']"';
                }.bind(this)).join(" ");

                var template = '<' + this.component + params + '></' + this.component + '>';

                var compiled = $compile(template)($scope);

                $element.append(compiled);
            }
        },
    ],
});
