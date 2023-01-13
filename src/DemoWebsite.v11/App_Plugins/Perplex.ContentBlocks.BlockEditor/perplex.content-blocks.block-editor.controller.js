angular.module("perplexContentBlocks").controller("Perplex.ContentBlocks.BlockEditor.Controller", [
    "$scope", "blockEditorService",
    function perplexContentBlocksBlockEditorController($scope, blockEditorService) {        
        var $ctrl = this;

        if (Object.prototype.toString.call($scope.model.value) !== "[object Object]") {
            $scope.model.value = {
                version: 4,
                header: {},
                blocks: {},
            };
        }

        var blockConfig = [{
            contentElementTypeKey: "7a84d7fc-dcf7-466c-b52e-bc59b237949e",
            label: "{{title}}",
        }];

        $ctrl.layout = null;        
        $ctrl.blocks = [];

        var blockEditor = blockEditorService.createModelObject($scope.model.value.blocks, $scope.model.editor, blockConfig, $scope, $scope);
        blockEditor.load().then(function() {
            $ctrl.layout = blockEditor.getLayout([]);
            updateBlocksFromLayout();
        });

        function updateBlocksFromLayout() {
            $ctrl.blocks = $ctrl.layout.map(blockEditor.getBlockObject.bind(blockEditor)).filter(function(block) {
                return block != null;
            });
        }

        $ctrl.addBlock = function() {
            var layoutEntry = blockEditor.create(blockConfig[0].contentElementTypeKey);
            if (layoutEntry == null) {
                // ???
                return;
            }

            layoutEntry.layoutId = "layout-1"; // GUID of first available layout
            layoutEntry.disabled = false;
            $ctrl.layout.push(layoutEntry);
            var block = blockEditor.getBlockObject(layoutEntry);
            $ctrl.blocks.push(block);
        }

        $ctrl.removeBlock = function(udi) {
            removeLayout($ctrl.layout, udi);
            removeBlock($ctrl.blocks, udi);
            removeData(udi);
        }

        function removeLayout(layouts, udi) {
            var layoutIdx = _.findIndex(layouts, function(layout) {
                return layout.contentUdi === udi;
            });

            if (layoutIdx > -1) {
                layouts.splice(layoutIdx, 1);
            }
        }

        function removeBlock(blocks, udi) {
            var blockIdx = _.findIndex(blocks, function(block) {
                return block.layout.contentUdi === udi;
            });

            if (blockIdx > -1) {
                blocks.splice(blockIdx, 1);
            }
        }

        function removeData(udi) {
            var contentIsUsed = Object.keys($scope.model.value.layout).some(function(editor) {
                return $scope.model.value.layout[editor].some(function(layout) {
                    return layout.contentUdi === udi;
                });
            });

            if (!contentIsUsed) {
                blockEditor.removeDataByUdi(udi);
            }
        }

        $ctrl.transferBlocks = function transferBlocks(from, to) {
            if (!Array.isArray(from)) {
                return;
            }

            from.forEach(function(entry) {
                var contains = to.some(function(item) {
                    return item.contentUdi === entry.contentUdi;
                });

                if (!contains) {
                    to.push(entry);
                }
            });

            updateBlocksFromLayout();
        }

        $ctrl.removeAll = function(editor, layouts) {
            if (editor === $scope.model.editor) {
                var i = layouts.length;
                while (i--) {
                    $ctrl.removeBlock(layouts[i].contentUdi);
                }

                updateBlocksFromLayout();
            } else {
                var i = layouts.length;
                while (i--) {
                    var layout = layouts[i];
                    removeLayout(layouts, layout.contentUdi);
                    removeData(layout.contentUdi);
                }
            }
        }
    }
]);

