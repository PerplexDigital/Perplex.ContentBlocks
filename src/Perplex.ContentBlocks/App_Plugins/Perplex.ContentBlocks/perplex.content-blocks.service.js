angular.module("perplexContentBlocks").service("Perplex.ContentBlocks.Service", [
    function () {
        /**
         * Creates copies of the header + blocks in the given data object
         * @param {any} data - Format { header: ..., blocks: [ ... ] }
         */
        function copyAll(data) {
            var copy = {};

            if (data.header) {
                copy.header = copyContentBlock(data.header);
            }

            if (Array.isArray(data.blocks)) {
                copy.blocks = [];

                for (var i = 0; i < data.blocks.length; i++) {
                    copy.blocks[i] = copyContentBlock(data.blocks[i]);
                }
            }

            return copy;
        }

        function copyContentBlock(original) {
            var copy = angular.copy(original);

            // Always generate a new unique id
            copy.id = String.CreateGuid();

            if (Array.isArray(copy.content) && copy.content[0] && copy.content[0].key) {
                // A Nested Content has its own unique id and caches an item based on it.
                // When we have create a copy we should therefore also update their Nested Content
                // id to prevent getting old cached values from Nested Content.
                copy.content[0].key = String.CreateGuid();
            }

            return copy;
        }

        this.copyContentBlock = copyContentBlock;
        this.copyAll = copyAll;
    }
]);
