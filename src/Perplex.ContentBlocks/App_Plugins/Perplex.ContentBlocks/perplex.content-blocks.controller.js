angular.module("perplexContentBlocks").controller("Perplex.ContentBlocks.Controller", [
    "$scope", "$sce", "$element", "$q", "editorState", "eventsService", "$timeout",
    "Perplex.ContentBlocks.Api", "Perplex.ContentBlocks.CopyPaste.Service", "notificationsService",
    function ($scope, $sce, $rootElement, $q, editorState, eventsService, $timeout, api, copyPasteService, notificationsService) {
        var vm = this;

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
                    confirmCallback: null
                },

                blocks: {
                    // block id => true/false
                    expanded: {},

                    // block id => true/false
                    showSettings: {},

                    // block id => true/false
                    loaded: {},
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
                previewIframeDesktop: null,
                previewIframeMobile: null,
                previewIframeFrameMobile: null,
                previewIframeFrameDesktop: null,

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
            }
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
                this.editorState.init();

                if (state.pageId == null) {
                    // We zitten niet in content
                    return;
                }

                this.initModelValue();
                this.copyPaste.init();
                this.setContainingGroupCssClass();

                var self = this;

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

                    $timeout(function () {
                        self.initDom();
                        self.initEvents();
                        self.preview.init();
                    });
                });
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
                state.dom.editorsContainer = document.querySelector(".umb-editor-container");

                var sidebarHolder = document.querySelector(".perplex-content-blocks .p-sidebar__holder");
                state.dom.previewColumn = sidebarHolder == null ? null : sidebarHolder.parentNode;
                state.dom.blocksContainer = document.querySelector(".perplex-content-blocks .p-editor");
                state.dom.contentBlocksViewport = document.querySelector(".umb-editor-container.umb-scrollable");
                state.dom.leftColumn = document.getElementById("leftcolumn");

                if (state.dom.previewColumn != null) {
                    state.dom.previewIframeDesktop = state.dom.previewColumn.querySelector(".p-sidebar__preview__root--desktop");
                    state.dom.previewIframeFrameDesktop = state.dom.previewColumn.querySelector(".p-sidebar__preview__frame--desktop");
                    state.dom.previewIframeMobile = state.dom.previewColumn.querySelector(".p-sidebar__preview__root--mobile");
                    state.dom.previewIframeFrameMobile = state.dom.previewColumn.querySelector(".p-sidebar__preview__frame--mobile");
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
                fn.preview.initEvents();
            },

            initHotkeys: function () {
                function handleEscape() {
                    if (!state.ui.picker.open && !state.ui.layoutPicker.open) {
                        return;
                    }

                    // Prioriteit: eerst layoutpicker sluiten
                    // en als die niet open is dan eventueel de blockpicker.
                    // In een $timeout zodat de wijzigingen door Angular worden verwerkt
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
                findValue: function (obj, props) {
                    if (typeof obj !== "object" || obj == null) {
                        return null;
                    }

                    if (!Array.isArray(props) || props.length === 0) {
                        return null;
                    }

                    var prop = props.shift();
                    var value = obj[prop];
                    if (value === undefined) {
                        return null;
                    }

                    if (props.length === 0) {
                        return value;
                    } else {
                        return this.findValue(value, props);
                    }
                },

                getContentBlockVisibleRatio: function (element) {
                    function getContentBlockVisibleRatio(element) {
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
                    }

                    function getContentBlocksViewport() {
                        var viewport = state.dom.contentBlocksViewport;
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

                    return getContentBlockVisibleRatio(element);
                },

                debounce: function debounce(func, wait) {
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
                },

                getCurrentCulture: function () {
                    var es = editorState.current;

                    if (es == null || !Array.isArray(es.variants)) {
                        return null;
                    }

                    var activeVariant = _.find(es.variants, function (variant) {
                        return variant.active;
                    });

                    if (activeVariant !== null && activeVariant.language != null) {
                        return activeVariant.language.culture;
                    }

                    return null;
                }
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
                        notificationsService.info("Copied " + name);
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
                    var data = {
                        header: $scope.model.value.header,
                        blocks: $scope.model.value.blocks
                    };

                    copyPasteService.copyAll(data);
                    notificationsService.info("Copied all blocks");
                },

                paste: function (afterBlockId) {
                    copyPasteService.pasteAll(function (header, blocks) {
                        if (header != null) {
                            if ($scope.model.value.header != null) {
                                notificationsService.warning("Cannot paste a header on a page with another header. If the header should be replaced, remove it first.");

                                // Do not paste blocks either, just stop.
                                return;
                            } else {
                                var definition = computed.definitionsById[header.definitionId];
                                if (definition == null) {
                                    // Header definition not found, either because unavailable for this page or removed in general.
                                    notificationsService.warning("The copied header is not available on this page and will be skipped.");
                                } else {
                                    $scope.model.value.header = header;
                                }
                            }
                        }

                        if (blocks != null) {
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
                                    notificationsService.warning("1 copied block is not available on this page and is skipped.");
                                } else {
                                    notificationsService.warning(skippedBlocks + " copied blocks are not available for this page and are skipped.");
                                }
                            }

                            var args = [idx, 0].concat(availableBlocks);
                            [].splice.apply($scope.model.value.blocks, args);

                            // Open and load all blocks
                            var numBlocks = availableBlocks.length;
                            for (var i = 0; i < numBlocks; i++) {
                                var block = availableBlocks[i];

                                // Only when pasting a single block -- immediately expand it
                                if (numBlocks === 1) {
                                    fn.blocks.openAndLoad(block.id);
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

                    fn.ui.fixOverlayStyling();
                }
            },

            updatePreviews: function () {
                fn.preview.updateDesktop();
                fn.preview.updateMobile();
            },

            getModelVersion: function () {
                return $scope.model.value.version;
            },

            setModelVersion: function (version) {
                $scope.model.value.version = version;
            },

            preview: {
                init: function () {
                    if (state.isNewPage) {
                        return;
                    }

                    var previewUrl = fn.preview.getPreviewUrl();
                    if (previewUrl != null) {
                        state.preview.previewUrl = $sce.trustAsResourceUrl(previewUrl);
                        state.preview.lastUpdate = Date.now();
                        $timeout(fn.preview.setPreviewScale);
                    }
                },

                initEvents: function () {
                    var debouncedSyncScroll = fn.utils.debounce(fn.preview.syncScroll, 500);
                    state.dom.editorsContainer.addEventListener("scroll", debouncedSyncScroll);
                    state.dom.editorsContainer.addEventListener("scroll", fn.preview.updatePreviewColumnPositionOnScroll);

                    var debouncedSetPreviewScale = fn.utils.debounce(fn.preview.setPreviewScale, 200);
                    window.addEventListener("resize", debouncedSetPreviewScale);

                    var unsubscribe = eventsService.on("content.saved", function () {
                        if (state.isNewPage) {
                            // Initialize previews
                            fn.editorState.init();
                            fn.preview.init();
                        } else {
                            // Update previews after save                    
                            fn.updatePreviews();
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
                    if (state.pageId == null) {
                        return null;
                    }

                    var root = "/umbraco/backoffice/api/contentblockspreviewapi/GetPreviewForIframe";
                    var qs = "?pageId=" + state.pageId + "&culture=" + (state.culture || "");

                    return root + qs;
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

                syncScroll: function () {
                    if (state.dom.previewIframeDesktop == null || state.dom.previewIframeDesktop.contentWindow == null) {
                        return;
                    }

                    var visibleBlockId = fn.blocks.getInViewBlockId(false) || fn.blocks.getInViewBlockId(true);

                    if (visibleBlockId == null || state.preview.visibleBlockId === visibleBlockId) {
                        return;
                    }

                    state.preview.visibleBlockId = visibleBlockId;
                    state.dom.previewIframeDesktop.contentWindow.postMessage({ blockId: state.preview.visibleBlockId }, window.location.origin);
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

                updateDesktop: function () {
                    this.updateIframe(state.dom.previewIframeDesktop);
                },

                updateMobile: function () {
                    this.updateIframe(state.dom.previewIframeMobile);
                },

                updateIframe: function (iframe) {
                    if (iframe == null) {
                        return;
                    }

                    iframe.onload = function () {
                        // Clear visible block -- always scroll again
                        state.preview.visibleBlockId = null;
                        fn.preview.syncScroll();
                    }

                    iframe.contentWindow.location.reload();
                    state.preview.lastUpdate = Date.now();
                },

                setPreviewScale: function () {
                    fn.preview.setDesktopPreviewScale();
                    fn.preview.setMobilePreviewScale();
                },

                setDesktopPreviewScale: function () {
                    fn.preview.setIframeScale(state.dom.previewIframeFrameDesktop, state.dom.previewIframeDesktop);
                },

                setMobilePreviewScale: function () {
                    fn.preview.setIframeScale(state.dom.previewIframeFrameMobile, state.dom.previewIframeMobile);
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
                get: function (property) {
                    return $scope.model.value.header[property];
                },

                set: function (property, value) {
                    $scope.model.value.header[property] = value;
                },

                pick: function () {
                    function disabledSelector(category) {
                        return !category.isEnabledForHeaders;
                    }

                    fn.picker.init(function (definitionId, layoutId) {
                        delete $scope.model.value.header;

                        var layoutId = layoutId;

                        if (layoutId == null) {
                            var layouts = computed.layoutsByDefinitionId[definitionId];
                            if (layouts != null && layouts.length > 0) {
                                layoutId = layouts[0].Id;
                            }
                        }

                        $timeout(function () {
                            var id = String.CreateGuid();

                            $scope.model.value.header = {
                                id: id,
                                definitionId: definitionId,
                                layoutId: layoutId,
                                // Empty NestedContent model value
                                content: [],
                            };

                            // Expand immediately
                            fn.blocks.openAndLoad(id);
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
                get: function (block, property) {
                    return block[property];
                },

                set: function (block, property, value) {
                    block[property] = value;
                },

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

                        var id = String.CreateGuid();

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

                        $scope.model.value.blocks.splice(idx, 0, {
                            id: id,
                            definitionId: definitionId,
                            layoutId: layoutId,
                            // Empty NestedContent model value
                            content: [],
                        });

                        // Expand immediately
                        fn.blocks.openAndLoad(id);
                    }

                    fn.picker.init(selectBlockCallback, disabledSelector);

                    fn.picker.open();
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

                slide: function (blockId, up) {
                    var element = fn.blocks.getElement(blockId);
                    var $main = $(element).find(".p-block__main");
                    if ($main.length === 0) {
                        fn.blocks.setExpand(blockId, !up);
                    } else {
                        var slideFn = up ? $.fn.slideUp : $.fn.slideDown;
                        slideFn.call($main, "fast", function () {
                            fn.blocks.setExpand(blockId, !up);
                        });
                    }
                },

                slideToggle: function (blockId) {
                    if (state.ui.blocks.expanded[blockId]) {
                        fn.blocks.slideUp(blockId);
                    } else {
                        fn.blocks.slideDown(blockId);
                    }
                },

                slideUp: function (blockId) {
                    fn.blocks.slide(blockId, true);
                },

                slideDown: function (blockId) {
                    fn.blocks.slide(blockId, false);
                },

                openAndLoad: function (id) {
                    state.ui.blocks.expanded[id] = true;
                    state.ui.blocks.loaded[id] = true;
                },

                toggleDisable: function (block) {
                    block.isDisabled = !block.isDisabled;
                },

                toggleSettings: function (id) {
                    state.ui.blocks.showSettings[id] = !state.ui.blocks.showSettings[id];
                },

                shouldLoad: function (id) {
                    return state.ui.blocks.loaded[id];
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

                layouts: {
                    getLayoutIndex: function (block) {
                        if (block == null || block.layoutId == null || block.definitionId == null) {
                            return null;
                        }

                        var layouts = computed.layoutsByDefinitionId[block.definitionId];
                        if (layouts == null) {
                            return null;
                        }

                        return _.findIndex(layouts, function (layout) {
                            return layout.Id === block.layoutId;
                        });
                    },

                    slider: {
                        onAfterChange: function (block, slideIdx) {
                            var layout = fn.blocks.layouts.getLayout(block, slideIdx);
                            if (layout != null) {
                                block.layoutId = layout.Id;
                            }
                        }
                    },

                    getLayout: function (block, index) {
                        var layouts = computed.layoutsByDefinitionId[block.definitionId];
                        if (layouts == null || !Array.isArray(layouts)) {
                            return null;
                        }

                        return layouts[index];
                    },

                    setLayout: function (block, layoutIdx) {
                        var layout = this.getLayout(block, layoutIdx);
                        if (layout != null) {
                            block.layoutId = layout.Id;
                        }
                    },
                },

                toggleExpand: function (blockId) {
                    if (state.ui.blocks.expanded[blockId]) {
                        fn.blocks.collapse(blockId);
                    } else {
                        fn.blocks.expand(blockId);
                    }
                },

                expand: function (blockId) {
                    fn.blocks.setExpand(blockId, true);
                },

                collapse: function (blockId) {
                    fn.blocks.setExpand(blockId, false);
                },

                setExpand: function (blockId, expand) {
                    state.ui.blocks.expanded[blockId] = expand;

                    if (expand && !state.ui.blocks.loaded[blockId]) {
                        state.ui.blocks.loaded[blockId] = true;
                    }

                    $timeout(fn.preview.syncScroll, 0);
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
                            var skip = !state.ui.blocks.expanded[blockId];

                            if (skip) {
                                if (isLast) {
                                    return prev.id;
                                } else {
                                    continue;
                                }
                            }
                        }

                        var visibleRatio = fn.utils.getContentBlockVisibleRatio(block);

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
                            var id = String.CreateGuid();
                            $scope.model.value.header = {
                                id: id,
                                definitionId: state.preset.Header.DefinitionId,
                                layoutId: state.preset.Header.LayoutId,
                                presetId: state.preset.Header.Id
                            }
                        }

                        if ($scope.model.value.blocks == null || $scope.model.value.blocks.length === 0) {
                            // Only apply when there are no blocks on this page yet
                            fn.preset.eachBlock(function (block) {
                                if (block != null) {
                                    if (!Array.isArray($scope.model.value.blocks)) {
                                        $scope.model.value.blocks = [];
                                    }

                                    var id = String.CreateGuid();
                                    $scope.model.value.blocks.push({
                                        id: id,
                                        definitionId: block.DefinitionId,
                                        layoutId: block.LayoutId,
                                        presetId: block.Id
                                    });

                                    state.ui.blocks.expanded[id] = true;
                                    state.ui.blocks.loaded[id] = true;
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
                }
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

                    if (state.ui.expandAll) {
                        if (!skipHeader && $scope.model.value.header != null) {
                            fn.blocks.slideDown($scope.model.value.header.id);
                        }

                        fn.blocks.eachBlock(function (block) {
                            fn.blocks.slideDown(block.id);
                        });
                    } else {
                        if (!skipHeader && $scope.model.value.header != null) {
                            fn.blocks.slideUp($scope.model.value.header.id);
                        }

                        fn.blocks.eachBlock(function (block) {
                            fn.blocks.slideUp(block.id);
                        });
                    }
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
    }
]);
