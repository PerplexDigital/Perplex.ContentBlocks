angular.module("perplexContentBlocks").controller("perplexContentBlocksEditorLayoutController",[
    "$scope",
    perplexContentBlocksEditorLayoutController
])

function perplexContentBlocksEditorLayoutController($scope) {
    this.layouts = [
        { value: "all", label: "Header + blocks" },
        { value: "blocks", label: "Blocks only" },
        { value: "header", label: "Header only" },        
    ]

    if ($scope.model.value == null || $scope.model.value === "") {
        // Defaults to first if unset
        $scope.model.value = this.layouts[0].value;
    }
}
