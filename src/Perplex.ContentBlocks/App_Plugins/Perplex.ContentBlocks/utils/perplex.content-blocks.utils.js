angular.module("perplexContentBlocks").service("contentBlocksUtils", [
    "editorState",
    perplexContentBlocksUtils
]);

function perplexContentBlocksUtils(editorState) {
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

    function getContentBlockVisibleRatio(element, viewport) {
        var $element = $(element);
        if ($element.length === 0) {
            return;
        }

        var vp = getContentBlocksViewport();
        var ep = getElementPosition($element);

        if (ep.top > vp.bottom || vp.top > ep.bottom) {
            // Totally out of view
            return 0;
        }

        if (ep.top >= vp.top && ep.bottom <= vp.bottom) {
            // Totally in view
            return 1;
        }

        var visibleTop = Math.max(ep.top, vp.top);
        var visibleBottom = Math.min(ep.bottom, vp.bottom);

        var visibleHeight = visibleBottom - visibleTop;
        return visibleHeight / ep.height;

        function getContentBlocksViewport() {            
            var bcr = viewport.getBoundingClientRect();

            var top = bcr.top;
            var height = viewport.clientHeight;
            var bottom = top + height;
            var center = (top + height) / 2;

            return {
                top: top,
                bottom: bottom,
                height: height,
                center: center
            }
        }

        function getElementPosition($element) {
            var offset = $element.offset();
            var top = offset.top;
            var bottom = top + $element.outerHeight();
            var height = bottom - top;
            var center = top + height / 2;

            return {
                top: top,
                bottom: bottom,
                center: center,
                height: height,
                element: $element
            };
        }
    }

    function debounce(func, wait) {
        var timeout;
        return function () {
            var context = this, args = arguments;
            var later = function () {
                timeout = null;
                func.apply(context, args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    function getCurrentCulture() {
        var es = editorState.current;

        if (es == null || !Array.isArray(es.variants)) {
            return null;
        }

        var activeVariant = _.find(es.variants, function (variant) {
            return variant.active;
        });

        if (activeVariant != null && activeVariant.language != null) {
            return activeVariant.language.culture;
        }

        return null;
    }

    this.copyContentBlock = copyContentBlock;
    this.copyAll = copyAll;
    this.getContentBlockVisibleRatio = getContentBlockVisibleRatio;
    this.debounce = debounce;
    this.getCurrentCulture = getCurrentCulture;
}
