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

        /**
         * Copies the given Content Block and all its data
         * @param {any} original
         * @returns {object} Copied Content Block
         */
        function copyContentBlock(original) {
            var copy = angular.copy(original);

            // Always generate a new unique id
            copy.id = String.CreateGuid();

            if (Array.isArray(copy.content)) {
                // A Nested Content has its own unique id and caches an item based on it.
                // When we have create a copy we should therefore also update their Nested Content
                // id to prevent getting old cached values from Nested Content.
                copy.content.forEach(updateNestedContentKey);
            }

            /**
             * Updates the key of this Nested Content and all its inner Nested Content items
             * @param {object} nestedContent Nested Content item
             */
            function updateNestedContentKey(nestedContent) {
                if (nestedContent == null || nestedContent.key == null) {
                    return;
                }

                nestedContent.key = String.CreateGuid();

                // Also update any nested Nested Content items inside this nestedContent
                for (var property in nestedContent) {
                    if (!nestedContent.hasOwnProperty(property)) {
                        continue;
                    }

                    var value = nestedContent[property];
                    if (!isNestedContentValue(value)) {
                        continue;
                    }
                    
                    // Update keys of all items
                    value.forEach(updateNestedContentKey);                    
                }

                function isNestedContentValue(value) {
                    return Array.isArray(value) && value.length > 0 && value[0].key != null && value[0].ncContentTypeAlias != null;
                }
            }

            return copy;
        }

        this.copyContentBlock = copyContentBlock;
        this.copyAll = copyAll;
    }
]);
