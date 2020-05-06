angular.module("perplexContentBlocks").controller("Perplex.ContentBlocks.ConfigurationStructureController", [
    "$scope",
    contentBlocksConfigurationStructureController
])

function contentBlocksConfigurationStructureController($scope) {
    this.options = [        
        { value: "blocks,header", label: "Header + Blocks" },        
        { value: "blocks", label: "Blocks" },
        { value: "header", label: "Header" },        
    ]

    if ($scope.model.value == null || $scope.model.value === "") {
        // Defaults to first if unset
        this.updateModel(this.options[0].value);
    } else {
        this.selected = Object.keys($scope.model.value).filter(function (k) { return $scope.model.value[k] }).join(",");
    }

    this.updateModel = function (selected) {
        if (selected == null) {
            // Clear all selected values
            $scope.model.value = {};
        } else {
            // Sets flags of selected values on object.
            // e.g. { header: true, blocks: true }
            $scope.model.value = selected.split(",").reduce(function (o, v) { o[v] = true; return o; }, {});
        }
    }
}
