angular.module("umbraco").service("Perplex.ContentBlocks.Service", [
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

            // Altijd een nieuw uniek id instellen
            copy.id = Guid.NewGuid();

            if (Array.isArray(copy.content) && copy.content[0] && copy.content[0].key) {
                // Een Nested Content item heeft ook een uniek id en cached op basis daarvan content. 
                // Voor de kopie moeten we dus ook een nieuw id instellen als er al een Nested Content
                // item was aangemaakt.
                copy.content[0].key = Guid.NewGuid();
            }

            return copy;
        }

        this.copyContentBlock = copyContentBlock;
        this.copyAll = copyAll;
    }
]);
