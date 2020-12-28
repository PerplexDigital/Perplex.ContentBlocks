angular.module("perplexContentBlocks").controller("Perplex.ContentBlocks.Controller", [
    "$scope", "$element", "$q", "editorState", "eventsService", "$timeout",
    "contentBlocksApi", "contentBlocksUtils", "contentBlocksCopyPasteService", "notificationsService",
    "serverValidationManager", "localizationService",
    perplexContentBlocksController,
]);

function perplexContentBlocksController(
    $scope, $rootElement, $q, editorState, eventsService, $timeout,
    api, utils, copyPasteService, notificationsService, serverValidationManager, localizationService) {
    var vm = this;

    var config = $scope.model.config;

    var constants = {
        preview: {
            mode: {
                desktop: "desktop",
                mobile: "mobile"
            }
        }
    }

    var state = {
        // The current version of the model, useful for possible future model changes
        // if we have to transform data.
        version: 2,

        initialized: false,
        pageId: null,
        isNewPage: null,
        // The currently active culture, e.g. "en-US"
        culture: null,

        documentType: null,

        ui: {
            picker: {
                open: false,

                // Array of { category: <string>, blocks: [IContentBlockDefinition] }
                categories: null,

                // Callback with selected block definition Id
                callback: null,

                // Currently selected category Id
                selectedCategoryId: null,

                // Id of currently selected block definition
                selectedBlockId: null,

                // Callback to run when confirming selection
                confirmCallback: null,
            },

            layoutPicker: {
                open: false,

                // IContentBlockDefinition
                block: null,

                // [IContentBlockLayout]
                layouts: null,

                // Callback function with selected layoutId parameter
                callback: null,

                // Id of currently selected layout
                selectedLayoutId: null,

                // Callback to run when confirming selection
                confirmCallback: null,

                // True when slick slider has initialized
                sliderInitialized: false,

                // True when the slider with layout name + description has initialized
                textSliderInitialized: false,
            },

            expandAll: false,
            reorder: false,
        },

        // Array of IContentBlockDefinition
        definitions: [],

        // Array of IContentBlockCategory
        categories: [],

        // IContentBlocksPreset
        preset: null,

        dom: {
            // Container containing the Umbraco editors and is the scrolling element
            editorsContainer: null,

            previewColumn: null,
            previewIframe: null,
            previewIframeFrame: null,

            blocksContainer: null,
            contentBlocksViewport: null,

            leftColumn: null,

            // block id -> block dom element
            blocks: {}
        },

        preview: {
            visibleBlockId: null,
            previewUrl: null,
            // constants.preview.mode
            mode: constants.preview.mode.desktop,
            lastUpdate: null
        },

        copyPaste: {
            hasData: false
        },

        // blockId -> perplexContentBlockController
        blocks: {},

        // blockId -> callback function to run when a block with that id registers itself
        onBlockRegisterFns: {},

        // blockId => [validationMessage]
        validationMessages: {},
    };

    var computed = {
        // Array of { category: <string>, blocks: [IContentBlockDefinition] },
        // computed based on state.definitions and state.categories
        definitionsByCategory: [],

        // definitionId => [IContentBlockCategory]
        categoriesByDefinitionId: {},

        // definitionId => [IContentBlockLayout]
        layoutsByDefinitionId: {},

        // definitionId => IContentBlockDefinition
        definitionsById: {},

        // categoryId => IContentBlockCategory
        categoriesById: {},

        // presetId => true/false (based on state.preset)
        mandatoryBlocks: {}
    }

    var fn = {
        init: function () {
            fn.editorState.init();
            if (state.pageId == null) {
                // Not in content
                return;
            }

            fn.initModelValue();
            fn.copyPaste.init();
            fn.validation.init();

            if (config.hidePropertyGroupContainer) {
                fn.setContainingGroupCssClass();
            }
        },

        initData: function () {
            $q.all([
                api.getDefinitionsForPage(state.documentType, state.culture),
                api.getAllCategories(),
                api.getPresetForPage(state.documentType, state.culture),
            ]).then(function (responses) {
                state.definitions = responses[0].data;
                state.categories = responses[1].data;
                state.preset = responses[2].data;

                fn.preset.apply();
                fn.updateComputed();
            }).finally(function () {
                state.initialized = true;

                fn.initDom();
                fn.initEvents();
                fn.preview.init();
            });
        },

        validation: {
            init: function () {
                var propertyAlias = $scope.model.alias;

                // Note that this is NOT the same as state.culture, 
                // which is the culture of the current content variant.
                var propertyCulture = $scope.model.culture;

                var unsubscribe = serverValidationManager.subscribe(propertyAlias, propertyCulture, "", function (valid, errors) {
                    // Clear validationMessages
                    state.validationMessages = {};

                    if (!valid) {
                        errors.forEach(function (error) {
                            var match = error.fieldName.match(/#content-blocks-id:([^#]+)#/);
                            if (match != null && match.length === 2) {
                                var blockId = match[1];
                                var errorMessage = error.errorMsg;
                                state.validationMessages[blockId] = state.validationMessages[blockId] || [];
                                state.validationMessages[blockId].push(errorMessage);
                            }
                        });
                    }
                });

                $scope.$on("$destroy", unsubscribe);
            }
        },

        editorState: {
            init: function () {
                var es = editorState.current;

                if (es == null) {
                    return;
                }

                state.pageId = es.id;
                state.isNewPage = state.pageId === 0;
                state.culture = fn.utils.getCurrentCulture();
                state.documentType = es.contentTypeAlias;

                if (state.isNewPage) {
                    var unsubscribe;
                    unsubscribe = eventsService.on("content.saved", function () {
                        if (state.isNewPage) {
                            // Update editorState with pageId / isNewPage etc.
                            fn.editorState.init();
                            unsubscribe();
                        }
                    });

                    $scope.$on("$destroy", unsubscribe);
                }
            }
        },

        initModelValue: function () {
            if (
                $scope.model.value == null ||
                Object.prototype.toString.call($scope.model.value) !== "[object Object]"
            ) {
                $scope.model.value = {
                    version: state.version,

                    // IContentBlock
                    header: null,

                    // Array of IContentBlock
                    blocks: []
                };
            }

            this.upgradeVersion();
        },

        initDom: function () {
            if ($rootElement.length === 0) {
                return;
            }

            var element = $rootElement[0];

            state.dom.editorsContainer = document.querySelector(".umb-editor-container");

            var sidebarHolder = element.querySelector(".p-sidebar__holder");
            state.dom.previewColumn = sidebarHolder == null ? null : sidebarHolder.parentNode;
            state.dom.blocksContainer = element.querySelector(".p-editor");
            state.dom.contentBlocksViewport = document.querySelector(".umb-editor-container.umb-scrollable");
            state.dom.leftColumn = document.getElementById("leftcolumn");

            if (state.dom.previewColumn != null) {
                state.dom.previewIframe = state.dom.previewColumn.querySelector(".p-sidebar__preview__root");
                state.dom.previewIframeFrame = state.dom.previewColumn.querySelector(".p-sidebar__preview__frame");
            }

            /* Fix Safari bugs */
            if (navigator.userAgent.indexOf('Safari') !== -1 && navigator.userAgent.indexOf('Mac') !== -1 && navigator.userAgent.indexOf('Chrome') === -1) {
                $('html').addClass('safari-mac'); // provide a class for the safari-mac specific css to filter with
            }
        },

        initEvents: function () {
            if (state.dom.editorsContainer == null) {
                return;
            }

            fn.initHotkeys();
        },

        initHotkeys: function () {
            function handleEscape() {
                if (!state.ui.picker.open && !state.ui.layoutPicker.open) {
                    return;
                }

                // Close the overlays in this order:
                // 1) Layout picker
                // 2) Block picker    
                // This order is important as the layout picker will be opened
                // from within the block picker. The close action should always close
                // the top level overlay first.
                $timeout(function () {
                    if (state.ui.layoutPicker.open) {
                        fn.layoutPicker.close();
                    } else if (state.ui.picker.open) {
                        fn.picker.close();
                    }
                });
            }

            function handleKeyup(e) {
                switch (e.keyCode) {
                    case 27: // Escape
                        return handleEscape(e);
                }
            }

            document.addEventListener("keyup", handleKeyup);

            $scope.$on("$destroy", function () {
                document.removeEventListener("keyup", handleKeyup);
            });
        },

        updateComputed: function () {
            computed.definitionsByCategory = _.map(state.categories, function (category) {
                return {
                    id: category.Id,
                    category: category.Name,
                    blocks: _.filter(state.definitions, function (block) {
                        return block.CategoryIds.indexOf(category.Id) > -1;
                    }),
                    isEnabledForHeaders: category.IsEnabledForHeaders,
                    isDisabledForBlocks: category.IsDisabledForBlocks,
                };
            });

            computed.categoriesById = _.reduce(state.categories, function (map, category) {
                map[category.Id] = category;
                return map;
            }, {});

            computed.categoriesByDefinitionId = _.reduce(state.definitions, function (map, block) {
                map[block.Id] = _.filter(_.map(block.CategoryIds || [], function (id) {
                    return computed.categoriesById[id];
                }), function (category) {
                    return category != null;
                });

                return map;
            }, {});

            computed.layoutsByDefinitionId = _.reduce(state.definitions, function (map, block) {
                map[block.Id] = block.Layouts || [];
                return map;
            }, {});

            computed.definitionsById = _.reduce(state.definitions, function (map, definition) {
                map[definition.Id] = definition;
                return map;
            }, {});

            computed.mandatoryBlocks = {};
            if (state.preset != null) {
                if (state.preset.Header != null && state.preset.Header.IsMandatory) {
                    computed.mandatoryBlocks[state.preset.Header.Id] = true;
                }

                fn.preset.eachBlock(function (block) {
                    if (block.IsMandatory) {
                        computed.mandatoryBlocks[block.Id] = true;
                    }
                });
            }
        },

        filters: {
            categoryHasBlocks: function (category) {
                return category != null && Array.isArray(category.blocks) && category.blocks.length > 0;
            }
        },

        save: function () {
            // Yeah...
            $("div[data-element='button-save'] > button").click();
        },

        utils: {
            getContentBlockVisibleRatio: utils.getContentBlockVisibleRatio,
            debounce: utils.debounce,
            getCurrentCulture: utils.getCurrentCulture,
        },

        picker: {
            init: function (callback, disabledSelector) {
                state.ui.picker.callback = callback;

                // Don't show hidden categories
                var definitionsByVisibleCategories = _.filter(computed.definitionsByCategory, function (c) {
                    return !computed.categoriesById[c.id].IsHidden;
                });

                state.ui.picker.categories = _.map(definitionsByVisibleCategories, function (category) {
                    var isDisabled = typeof disabledSelector === "function"
                        ? disabledSelector(category)
                        : false;

                    return Object.assign({}, category, { isDisabled: isDisabled });
                });

                var firstEnabled = _.find(state.ui.picker.categories, function (category) {
                    return !category.isDisabled && category.blocks.length > 0;
                });

                if (firstEnabled != null) {
                    state.ui.picker.selectedCategoryId = firstEnabled.id;
                }
            },

            pick: function (blockId) {
                state.ui.picker.selectedBlockId = blockId;

                state.ui.picker.confirmCallback = function () {
                    var callback = state.ui.picker.callback;
                    if (typeof callback === "function") {
                        callback(blockId);
                    }

                    fn.picker.close();
                }
            },

            confirm: function () {
                if (typeof state.ui.picker.confirmCallback === "function") {
                    state.ui.picker.confirmCallback();
                }
            },

            open: function () {
                state.ui.picker.open = true;
                fn.ui.fixOverlayStyling();
            },

            close: function () {
                state.ui.picker.open = false;

                state.ui.picker.selectedBlockId = null;
                state.ui.picker.selectedCategoryId = null;

                fn.ui.fixOverlayStyling();
            }
        },

        copyPaste: {
            init: function () {
                copyPasteService.onChange(function (data) {
                    $timeout(function () {
                        state.copyPaste.hasData = data != null;
                    });
                })
            },

            confirmCopy: function (block) {
                var name = fn.blocks.getBlockName(block);
                if (name != null) {
                    localizationService.localize("perplexContentBlocks_copied").then(function (value) {
                        notificationsService.info(value + " " + name);
                    });
                }
            },

            copyHeader: function (header) {
                if (header != null) {
                    copyPasteService.copyHeader(header);
                    this.confirmCopy(header);
                }
            },

            copyBlock: function (block) {
                if (block != null) {
                    copyPasteService.copyBlock(block);
                    this.confirmCopy(block);
                }
            },

            copyAll: function () {
                var data = {};

                if (config.structure.header) {
                    data.header = $scope.model.value.header;
                }

                if (config.structure.blocks) {
                    data.blocks = $scope.model.value.blocks;
                }

                copyPasteService.copyAll(data);
                localizationService.localize("perplexContentBlocks_copiedAllBlocks").then(function (value) {
                    notificationsService.info(value);
                });
            },

            paste: function (afterBlockId) {
                copyPasteService.pasteAll(function (header, blocks) {
                    if (header != null && config.structure.header) {
                        if ($scope.model.value.header != null) {
                            localizationService.localize("perplexContentBlocks_copiedHeaderNotInsertedHeaderIsPresent").then(function (value) {
                                notificationsService.warning(value);
                            });
                        } else {
                            var definition = computed.definitionsById[header.definitionId];
                            if (definition == null) {
                                // Header definition not found, either because unavailable for this page or removed in general.
                                localizationService.localize("perplexContentBlocks_copiedHeaderNotAvailable").then(function (value) {
                                    notificationsService.warning(value);
                                });
                            } else {
                                $scope.model.value.header = header;
                            }
                        }
                    }

                    if (blocks != null && config.structure.blocks) {
                        var idx = $scope.model.value.blocks.length - 1;
                        if (afterBlockId != null) {
                            if ($scope.model.value.header != null && $scope.model.value.header.id === afterBlockId) {
                                idx = 0;
                            } else {
                                var blockIdx = fn.blocks.getIndex(afterBlockId);
                                if (blockIdx > -1) {
                                    idx = blockIdx + 1;
                                }
                            }
                        }

                        // Add all blocks in 1 statement -- filtering out blocks that are unavailable for this page or any mandatory blocks

                        var availableBlocks = _.filter(blocks, function (block) {
                            return computed.definitionsById[block.definitionId] != null && !computed.mandatoryBlocks[block.presetId];
                        });

                        var skippedBlocks = blocks.length - availableBlocks.length;
                        if (skippedBlocks > 0) {
                            if (skippedBlocks === 1) {
                                localizationService.localize("perplexContentBlocks_copied1BlockNotAvailable").then(function (value) {
                                    notificationsService.warning(value);
                                });
                            } else {
                                localizationService.localize("perplexContentBlocks_copiedMultipleBlockNotAvailable").then(function (value) {
                                    notificationsService.warning(skippedBlocks + " " + value);
                                });
                            }
                        }

                        var args = [idx, 0].concat(availableBlocks);
                        [].splice.apply($scope.model.value.blocks, args);

                        // Open and load all blocks
                        var numBlocks = availableBlocks.length;
                        for (var i = 0; i < numBlocks; i++) {
                            var block = availableBlocks[i];

                            if (numBlocks === 1) {
                                // Only when pasting a single block -- immediately expand it
                                fn.blocks.withCtrl(block.id, function (block) {
                                    block.open();
                                });
                            }
                        }
                    }

                    // Remove copied data
                    copyPasteService.clear();
                });
            }
        },

        layoutPicker: {
            init: function (definitionId, blockCallback) {
                function selectLayoutCallback(layoutId) {
                    state.ui.layoutPicker.selectedLayoutId = layoutId;

                    state.ui.layoutPicker.confirmCallback = function () {
                        blockCallback(definitionId, layoutId);

                        fn.picker.close();
                        fn.layoutPicker.close();
                    }
                }

                var layouts = computed.layoutsByDefinitionId[definitionId];

                state.ui.layoutPicker.callback = selectLayoutCallback;
                state.ui.layoutPicker.layouts = layouts;
                state.ui.layoutPicker.block = computed.definitionsById[definitionId];

                this.open();
            },

            pick: function (layoutId) {
                var callback = state.ui.layoutPicker.callback;
                if (typeof callback === "function") {
                    callback(layoutId);
                }
            },

            confirm: function () {
                if (typeof state.ui.layoutPicker.confirmCallback === "function") {
                    state.ui.layoutPicker.confirmCallback();
                }
            },

            open: function () {
                state.ui.layoutPicker.open = true;

                fn.ui.fixOverlayStyling();
            },

            close: function () {
                state.ui.layoutPicker.open = false;
                state.ui.layoutPicker.selectedLayoutId = null;
                state.ui.layoutPicker.sliderInitialized = false;
                state.ui.layoutPicker.textSliderInitialized = false;

                fn.ui.fixOverlayStyling();
            }
        },

        getModelVersion: function () {
            return $scope.model.value.version;
        },

        setModelVersion: function (version) {
            $scope.model.value.version = version;
        },

        preview: {
            init: function () {
                if (config.disablePreview) {
                    return;
                }

                fn.preview.initEvents();
                fn.preview.updatePreview();
            },

            initEvents: function () {
                var debouncedSyncScroll = fn.utils.debounce(fn.preview.syncScroll, 500);
                state.dom.editorsContainer.addEventListener("scroll", debouncedSyncScroll);
                state.dom.editorsContainer.addEventListener("scroll", fn.preview.updatePreviewColumnPositionOnScroll);

                var debouncedSetPreviewScale = fn.utils.debounce(fn.preview.setPreviewScale, 200);
                window.addEventListener("resize", debouncedSetPreviewScale);

                var unsubscribe = eventsService.on("content.saved", function () {
                    if (state.isNewPage) {
                        fn.preview.init();
                    } else {
                        // Update previews after save                    
                        fn.preview.updatePreview();
                    }
                });

                this.setPreviewScaleOnLeftColumnResize(debouncedSetPreviewScale);

                $scope.$on("$destroy", function () {
                    state.dom.editorsContainer.removeEventListener("scroll", debouncedSyncScroll);
                    state.dom.editorsContainer.removeEventListener("scroll", fn.preview.updatePreviewColumnPositionOnScroll);
                    window.removeEventListener("resize", debouncedSetPreviewScale);

                    if (typeof unsubscribe === "function") {
                        unsubscribe();
                    }
                });
            },

            getPreviewUrl: function () {
                if (state.pageId == null || state.pageId === 0) {
                    return null;
                }

                var endpoint = "/umbraco/backoffice/api/contentblockspreviewapi/GetPreviewForIframe";
                var qs = "?pageId=" + state.pageId + "&culture=" + (state.culture || "");

                // Return absolute URL
                return window.location.origin + endpoint + qs;
            },

            setPreviewUrl: function () {
                var previewUrl = fn.preview.getPreviewUrl();
                if (previewUrl != null) {
                    state.preview.previewUrl = previewUrl;
                }
            },

            updatePreview: function () {
                if (state.preview.previewUrl == null) {
                    fn.preview.setPreviewUrl();
                    // In $timeout to run after iframe DOM element has initialized
                    // for new pages that have not been saved yet.
                    $timeout(fn.preview.setPreviewScale);
                }

                fn.preview.updateIframe(state.dom.previewIframe);
            },

            setPreviewScaleOnLeftColumnResize: function (debouncedSetPreviewScale) {
                if (typeof MutationObserver === "function" && state.dom.leftColumn != null) {
                    var navigationElement = state.dom.leftColumn.children[0];
                    if (navigationElement != null) {
                        var observer = new MutationObserver(function onMutation(mutations) {
                            debouncedSetPreviewScale();
                        });

                        // Start observing
                        observer.observe(navigationElement, {
                            attributes: true,
                            // Only look at changes to the "style" attribute
                            attributeFilter: ["style"]
                        });

                        $scope.$on("$destroy", function () {
                            observer.disconnect();
                        });

                        return observer;
                    }
                }
            },

            syncScroll: function (ignoreCollapsed) {
                if (state.dom.previewIframe == null || state.dom.previewIframe.contentWindow == null) {
                    return;
                }

                var visibleBlockId = fn.blocks.getInViewBlockId(false);

                if (visibleBlockId == null && !ignoreCollapsed) {
                    visibleBlockId = fn.blocks.getInViewBlockId(true);
                }

                if (visibleBlockId == null || state.preview.visibleBlockId === visibleBlockId) {
                    return;
                }

                state.preview.visibleBlockId = visibleBlockId;
                state.dom.previewIframe.contentWindow.postMessage({ blockId: state.preview.visibleBlockId }, window.location.origin);
            },

            updatePreviewColumnPositionOnScroll: function (e) {
                if (e.srcElement == null) {
                    return;
                }

                fn.preview.updatePreviewColumnPosition(e.srcElement.scrollTop);
            },

            updatePreviewColumnPosition: function (scrollTop) {
                var blocksRect = state.dom.blocksContainer.getBoundingClientRect();
                var previewRect = state.dom.previewColumn.getBoundingClientRect();
                var editorsRect = state.dom.editorsContainer.getBoundingClientRect();

                var distanceToTop = blocksRect.top - editorsRect.top
                var offset = scrollTop + distanceToTop;
                var padding = 20;
                var actualScroll = scrollTop - offset + padding;

                var blocksTop = blocksRect.top;
                var blocksPosY = blocksTop + blocksRect.height;
                var previewTop = previewRect.top;
                var previewPosY = previewTop + previewRect.height;

                var previewMargin = parseInt(state.dom.previewColumn.style.marginTop) || 0;
                var toMove = actualScroll - previewMargin;

                if (previewRect.height > blocksRect.height) {
                    // If the preview container is longer than the blocks container,
                    // align it to the top of the screen regardless of the current scroll
                    toMove = blocksTop - previewTop;
                } else {
                    if ((previewPosY + toMove) >= blocksPosY) {
                        // When the bottom of the preview container
                        // goes beyond the bottom of the blocks container 
                        // -> set to bottom
                        var maxMove = blocksPosY - previewPosY;
                        toMove = Math.max(0, maxMove);
                    } else if (actualScroll < 0 || previewTop + toMove < blocksTop) {
                        toMove = blocksTop - previewTop;
                    }
                }

                if (state.dom.previewColumn != null) {
                    state.dom.previewColumn.style.marginTop = (previewMargin + toMove) + "px";
                }
            },

            switchTo: function (mode) {
                if (state.preview.mode === mode) {
                    return;
                }

                state.preview.mode = mode;
                $timeout(fn.preview.setPreviewScale);
            },

            switchToDesktop: function () {
                fn.preview.switchTo(constants.preview.mode.desktop);
            },

            switchToMobile: function () {
                fn.preview.switchTo(constants.preview.mode.mobile);
            },

            updateIframe: function (iframe) {
                if (iframe == null || state.preview.previewUrl == null) {
                    return;
                }

                if (typeof iframe.onload !== "function") {
                    iframe.onload = function () {
                        // Clear visible block -- always scroll again
                        state.preview.visibleBlockId = null;
                        fn.preview.syncScroll();
                    }
                }

                if (iframe.src === state.preview.previewUrl) {
                    // Reload existing iframe to maintain scroll position
                    iframe.contentWindow.location.reload();
                } else {
                    // Load the iframe
                    iframe.src = state.preview.previewUrl;
                }

                state.preview.lastUpdate = Date.now();
            },

            setPreviewScale: function () {
                fn.preview.setIframeScale(state.dom.previewIframeFrame, state.dom.previewIframe);
            },

            setIframeScale: function (iframeFrame, iframe) {
                if (iframeFrame == null || iframe == null) {
                    return;
                }

                var ratio = iframeFrame.clientWidth / iframe.clientWidth;
                iframe.style.transform = "scale(" + ratio + ") translateZ(0)";
            }
        },

        header: {
            pick: function () {
                function disabledSelector(category) {
                    return !category.isEnabledForHeaders;
                }

                fn.picker.init(function (definitionId, layoutId) {
                    var layoutId = layoutId;

                    if (layoutId == null) {
                        var layouts = computed.layoutsByDefinitionId[definitionId];
                        if (layouts != null && layouts.length > 0) {
                            layoutId = layouts[0].Id;
                        }
                    }

                    $scope.model.value.header = fn.blocks.createEmpty(definitionId, layoutId);

                    fn.blocks.withCtrl($scope.model.value.header.id, function (block) {
                        // Open block immediately
                        block.open();
                    });
                }, disabledSelector);

                fn.picker.open();
            },

            remove: function () {
                if ($scope.model.value.header == null) {
                    return;
                }

                $scope.model.value.header = null;
            }
        },

        blocks: {
            add: function (afterBlockId) {
                if (!Array.isArray($scope.model.value.blocks)) {
                    $scope.model.value.blocks = [];
                }

                function disabledSelector(category) {
                    return category.isDisabledForBlocks;
                }

                function selectBlockCallback(definitionId, layoutId) {
                    var layoutId = layoutId;

                    if (layoutId == null) {
                        var layouts = computed.layoutsByDefinitionId[definitionId];
                        if (layouts != null && layouts.length > 0) {
                            layoutId = layouts[0].Id;
                        }
                    }

                    // Add at the end by default
                    var idx = $scope.model.value.blocks.length - 1;

                    if (afterBlockId != null) {
                        if ($scope.model.value.header != null && $scope.model.value.header.id === afterBlockId) {
                            idx = 0;
                        } else {
                            var blockIdx = fn.blocks.getIndex(afterBlockId);
                            if (blockIdx > -1) {
                                idx = blockIdx + 1;
                            }
                        }
                    }

                    var empty = fn.blocks.createEmpty(definitionId, layoutId);
                    $scope.model.value.blocks.splice(idx, 0, empty);

                    fn.blocks.withCtrl(empty.id, function (block) {
                        // Open immediately
                        block.open();
                    });
                }

                fn.picker.init(selectBlockCallback, disabledSelector);

                fn.picker.open();
            },

            createEmpty: function (definitionId, layoutId) {
                var id = String.CreateGuid();

                return {
                    id: id,
                    definitionId: definitionId,
                    layoutId: layoutId,
                    // Empty NestedContent model value
                    content: [],
                };
            },

            remove: function (id) {
                if (!Array.isArray($scope.model.value.blocks)) {
                    return;
                }

                var idx = fn.blocks.getIndex(id);
                if (idx > -1) {
                    $scope.model.value.blocks.splice(idx, 1);
                }
            },

            registerElement: function (blockId, $element) {
                if (blockId == null) {
                    throw new Error("Block id should not be null / undefined!");
                }

                if ($element.length === 1) {
                    var element = $element[0];

                    state.dom.blocks[blockId] = element;

                    return function removeElement() {
                        delete state.dom.blocks[blockId];
                    }
                }
            },

            getElement: function (id) {
                return state.dom.blocks[id];
            },

            registerBlockController: function (id, controller) {
                state.blocks[id] = controller;

                var onRegister = state.onBlockRegisterFns[id];
                if (typeof onRegister === "function") {
                    onRegister(controller);
                }

                return function deregisterController() {
                    delete state.blocks[id];
                }
            },

            /**
             * Calls `callback` with the controller of the block with the given id.
             * If the block controller has not been registered yet, the callback will be called
             * when it does. Otherwise it is called immediately.
             * @param {any} id Block id
             * @param {any} callback Callback function to be called when the controller of the block is available
             */
            withCtrl: function (id, callback) {
                var block = state.blocks[id];
                if (block != null) {
                    callback(block);
                } else {
                    state.onBlockRegisterFns[id] = function (block) {
                        callback(block);
                        delete state.onBlockRegisterFns[id];
                    }
                }
            },

            getIndex: function (id) {
                return _.findIndex($scope.model.value.blocks, function (block) {
                    return block.id === id;
                })
            },

            getBlockName: function (block) {
                if (block == null) {
                    return null;
                }

                var definition = computed.definitionsById[block.definitionId];
                if (definition == null) {
                    return null;
                }

                return definition.Name;
            },

            eachBlock: function (callback) {
                if (Array.isArray($scope.model.value.blocks)) {
                    for (var i = 0; i < $scope.model.value.blocks.length; i++) {
                        var block = $scope.model.value.blocks[i];
                        callback(block);
                    }
                }
            },

            getInViewBlockId: function (includeCollapsed) {
                var blocks = state.dom.blocksContainer.querySelectorAll(".p-block__item");

                var prev = {
                    id: null,
                    ratio: null,
                }

                for (var i = 0; i < blocks.length; i++) {
                    var block = blocks[i];
                    var blockId = block.dataset.id;
                    var isLast = i === blocks.length - 1;

                    if (!includeCollapsed) {
                        var blockCtrl = state.blocks[blockId]

                        var skip = blockCtrl != null && !blockCtrl.state.open;

                        if (skip) {
                            if (isLast) {
                                return prev.id;
                            } else {
                                continue;
                            }
                        }
                    }

                    var visibleRatio = fn.utils.getContentBlockVisibleRatio(block, state.dom.contentBlocksViewport);

                    if (visibleRatio === 1) {
                        // Completely visible
                        return block.dataset.id;
                    }

                    if (visibleRatio === 0) {
                        if (prev.id != null) {
                            return prev.id;
                        }
                    }

                    if (visibleRatio > 0 && visibleRatio < 1) {
                        if (prev.id != null && prev.ratio > visibleRatio) {
                            return prev.id;
                        } else if (isLast) {
                            // Last block -> show this block
                            return blockId;
                        } else {
                            // Store for later
                            prev = { id: blockId, ratio: visibleRatio };
                        }
                    }
                }
            },
        },

        upgradeVersion: function () {
            // Add any version upgrade logic here,
            // e.g. if(fn.getModelVersion() === 1 && state.version === 2) { ... }

            if (fn.getModelVersion() < 2) {
                fn.versionUpgrades.addGuids();
            }

            fn.setModelVersion(state.version);
        },

        setContainingGroupCssClass: function () {
            if ($rootElement == null) {
                return;
            }

            var propertyContainer = $rootElement.closest(".umb-property-editor");
            var isNestedProperty = propertyContainer.parent().closest(".umb-property-editor").length > 0;
            if (isNestedProperty) {
                // Do not hide the containing property group if we 
                // are nested in some other property editor like NestedContent.
                return;
            }

            var tabGroup = $rootElement.closest(".umb-group-panel");
            if (tabGroup != null) {
                tabGroup.addClass("perplex-content-blocks__panel");
            }
        },

        preset: {
            apply: function () {
                if (state.preset != null) {
                    if ($scope.model.value.header == null && state.preset.Header != null) {
                        // Only apply when there is no header yet on this page
                        var block = fn.preset.createBlock(state.preset.Header);
                        $scope.model.value.header = block;
                        fn.blocks.withCtrl(block.id, function (blockCtrl) {
                            blockCtrl.open();
                        });
                    }

                    if ($scope.model.value.blocks == null || $scope.model.value.blocks.length === 0) {
                        // Only apply when there are no blocks on this page yet
                        fn.preset.eachBlock(function (preset) {
                            if (preset != null) {
                                if (!Array.isArray($scope.model.value.blocks)) {
                                    $scope.model.value.blocks = [];
                                }

                                var block = fn.preset.createBlock(preset);
                                $scope.model.value.blocks.push(block);
                                fn.blocks.withCtrl(block.id, function (blockCtrl) {
                                    blockCtrl.open();
                                });
                            }
                        });
                    }
                }
            },

            eachBlock: function (callback) {
                if (Array.isArray(state.preset.Blocks)) {
                    for (var i = 0; i < state.preset.Blocks.length; i++) {
                        var block = state.preset.Blocks[i];
                        callback(block);
                    }
                }
            },

            createBlock: function (preset) {
                var emptyBlock = fn.blocks.createEmpty(preset.DefinitionId, preset.LayoutId);
                emptyBlock.presetId = preset.Id;
                return emptyBlock;
            },
        },

        versionUpgrades: {
            addGuids: function () {
                if ($scope.model.value == null) {
                    return;
                }

                // Header
                if ($scope.model.value.header != null && $scope.model.value.header.id == null) {
                    $scope.model.value.header.id = String.CreateGuid();
                }

                // Blocks
                fn.blocks.eachBlock(function (block) {
                    if (block != null && block.id == null) {
                        block.id = String.CreateGuid();
                    }
                })
            }
        },

        ui: {
            toggleExpandAll: function () {
                fn.ui.setExpandAll(!state.ui.expandAll);
            },

            setExpandAll: function (expandAll, skipHeader) {
                state.ui.expandAll = !!expandAll;

                if (!skipHeader && $scope.model.value.header != null && config.structure.header) {
                    fn.blocks.withCtrl($scope.model.value.header.id, slideFn);
                }

                if (config.structure.blocks) {
                    fn.blocks.eachBlock(function (block) {
                        fn.blocks.withCtrl(block.id, slideFn);
                    });
                }

                function slideFn(block) {
                    if (state.ui.expandAll) {
                        block.open();
                    } else {
                        block.close();
                    }
                }
            },

            closeOtherBlockSettings: function (exceptId) {
                Object.keys(state.blocks).forEach(function (id) {
                    if (id === exceptId) return;
                    var otherBlock = state.blocks[id];
                    if (otherBlock != null) {
                        otherBlock.closeSettings();
                    }
                });
            },

            toggleReorder: function () {
                state.ui.reorder = !state.ui.reorder;

                if (state.ui.reorder) {
                    fn.ui.setExpandAll(false, true);
                }
            },

            fixOverlayStyling: function () {
                if (state.dom.leftColumn == null || state.dom.contentBlocksViewport == null) {
                    return;
                }

                var overlayIsOpen = state.ui.picker.open || state.ui.layoutPicker.open;

                if (overlayIsOpen) {
                    // Set z-index of element to -1
                    state.dom.leftColumn.style.zIndex = -1;
                    state.dom.contentBlocksViewport.style.zIndex = 100;
                } else {
                    // Remove element z-index
                    state.dom.leftColumn.style.zIndex = null;
                    state.dom.contentBlocksViewport.style.zIndex = null;
                }
            },
        }
    };

    vm.state = state;
    vm.fn = fn;
    vm.computed = computed;
    vm.constants = constants;
    vm.config = config;

    fn.init();
}
