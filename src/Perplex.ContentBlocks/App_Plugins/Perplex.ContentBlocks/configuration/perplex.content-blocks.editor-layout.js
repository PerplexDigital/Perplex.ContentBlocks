angular.module("perplexContentBlocks").controller("perplexContentBlocksEditorLayoutController", [
    "$scope",
    perplexContentBlocksEditorLayoutController
])

function perplexContentBlocksEditorLayoutController($scope) {
    this.layouts = [
        { value: "blocks,header", label: "Header + Blocks" },
        { value: "blocks", label: "Blocks only" },
        { value: "header", label: "Header only" },
    ]

    if ($scope.model.value == null || $scope.model.value === "") {
        // Defaults to first if unset
        this.updateModel(this.layouts[0].value);
    } else {
        this.selectedLayout = Object.keys($scope.model.value).filter(function (k) { return $scope.model.value[k] }).join(",");
    }

    this.updateModel = function (selectedLayout) {
        if (selectedLayout == null) {
            // Clear all selected values
            $scope.model.value = {};
        } else {
            // Sets flags of selected values on object.
            // e.g. { header: true, blocks: true }
            $scope.model.value = selectedLayout.split(",").reduce(function (o, v) { o[v] = true; return o; }, {});
        }
    }
}
