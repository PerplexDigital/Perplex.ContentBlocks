angular.module("perplexContentBlocks").service("contentBlocksTabFocusService", [
    "$rootScope",
    "$timeout",
    tabFocusService
]);

function tabFocusService($rootScope, $timeout) {
    // Format: { id => { callbacks: Array<Function>, observer: <MutationObserver> } }
    var observedTabs = {};

    // Format: { id => { tabIds: Array<number>, observer: <MutationObserver> } }
    var observedContentApps = {};

    // Next unique DOM element id, is incremented on each use.
    var nextId = 1;

    // Key used for the dataset entry referencing the unique id for the DOM element
    var idKey = "tabFocusId";

    /**
     * Subscribes to tab focus events for the given $element
     * @param {*} $element jQuery element
     * @param {(unsubscribe:Function) => *} callback Callback function
     * @returns {Function}
     */
    function subscribe($element, callback) {
        var tab = getTab($element);
        var tabId = getElementId(tab);
        if (tab == null || tabId == null || !supportsMutationObserver()) {
            callback(angular.noop);
            // No need to unsubscribe here, as we did not subcribe
            return angular.noop;
        }

        var contentApp = getContentApp($(tab));
        var contentAppId = getElementId(contentApp)

        setupObservers(tab, tabId, contentApp, contentAppId, callback);

        function unsubscribe() {
            stopObserving(tabId, contentAppId, callback);
        }

        $timeout(function () {
            // Check if tab is active after next digest
            // to allow Angular to act on ng-show / ng-if etc.
            if (tabIsActive(tab, contentApp)) {
                // Tab was already active
                // Run callback and provide the unsubscribe
                // immediately as this function has not returned it yet.
                callback(unsubscribe);
            }
        });

        return unsubscribe;
    }

    /**
     * Returns whether the current browser supports MutationObserver
     * @returns {boolean}
     */
    function supportsMutationObserver() {
        return typeof window.MutationObserver === "function";
    }

    /**
     * Configures a MutationObserver to start watching the tab and call the
     * provided callback function when it becomes active.
     * @param {HtmlElement} tab The tab to observe
     * @param {Function} callback Callback to run when the tab becomes active
     * @returns {MutationObserver}
     */
    function createTabObserver(tab, contentApp, callback) {
        return createClassListObserver(tab, callback, function (mutation) {
            return (contentApp == null || contentAppIsActive(contentApp)) && tabWasActivated(mutation, tab, contentApp);
        });
    }

    /**
     * Configures a MutationObserver to start watching the Content App and call the
     * provided callback function when it becomes active.
     * @param {HtmlElement} tab The Content App to observe
     * @param {Function} callback Callback to run when the Content App becomes active
     * @returns {MutationObserver}
     */
    function createContentAppObserver(tab, contentApp, callback) {
        return createClassListObserver(contentApp, callback, function (mutation) {
            return contentAppWasActivated(mutation, contentApp) && tabIsActive(tab, contentApp);
        });
    }

    function createClassListObserver(target, callback, callbackTrigger) {
        var observer = new MutationObserver(function onMutation(mutations) {
            for (var i = 0; i < mutations.length; i++) {
                var mutation = mutations[i];

                if (callbackTrigger(mutation)) {
                    callback();
                }
            }
        });

        // Start observing
        observer.observe(target, {
            attributes: true,
            // Only look at changes to the "class" attribute
            attributeFilter: ["class"],
            // We need the old value to see how the attribute was changed
            attributeOldValue: true
        });

        return observer;
    }

    /**
     * Returns true if the tab is currently active
     * @param {HtmlElement} tab The tab to check
     * @returns {boolean}
     */
    function tabIsActive(tab, contentApp) {
        if (contentApp != null && !contentAppIsActive(contentApp)) {
            return false;
        }

        return !tab.classList.contains("ng-hide");
    }

    /**
     * Returns true if the content app is currently active
     * @param {HtmlElement} tab The tab to check
     * @returns {boolean}
     */
    function contentAppIsActive(contentApp) {
        return contentApp != null && !contentApp.classList.contains("ng-hide");
    }

    /**
     * Returns true if the tab was activated, based on information in the given MutationRecord
     * @param {MutationRecord} mutation A MutationRecord for the tab
     * @returns {boolean}
     */
    function tabWasActivated(mutation, tab, contentApp) {
        if (!tabIsActive(tab, contentApp)) {
            return false;
        }

        var tabWasActive = !/(^| )ng-hide( |$)/.test(mutation.oldValue);
        return !tabWasActive;
    }

    /**
     * Returns true if the content app was activated, based on information in the given MutationRecord
     * @param {MutationRecord} mutation A MutationRecord for the tab
     * @returns {boolean}
     */
    function contentAppWasActivated(mutation, contentApp) {
        if (!contentAppIsActive(contentApp)) {
            return false;
        }

        var contentAppWasActive = !/(^| )ng-hide( |$)/.test(mutation.oldValue);
        return !contentAppWasActive;
    }

    /**
     * Returns the element's unique tab focus id
     * @param {HtmlElement} element The element to get the id of
     * @returns {number}
     */
    function getElementId(element) {
        if (element == null) {
            return null;
        }

        var id = element.dataset[idKey];

        return id != null
            ? parseInt(id, 10)
            : setElementId(element);
    }

    /**
     * Sets a new unique id on the given element and returns the id that was set
     * @param {HTMLElement} element
     * @returns {number}
     */
    function setElementId(element) {
        element.dataset[idKey] = nextId;
        return nextId++;
    }

    /**
     * Returns the nearest tab of the given element
     * @param {*} $element jQuery Lite element
     * @returns {HTMLElement}
     */
    function getTab($element) {
        // Content Tab: .umb-group-panel__content
        // Content App: .umb-editor-sub-view__content
        return $element.closest(".umb-group-panel__content,.umb-editor-sub-view__content")[0];
    }

    /**
     * Returns the nearest Content App container of the given element
     * @param {*} $element jQuery Lite element
     * @returns {HTMLElement}
     */
    function getContentApp($element) {
        return $element.closest(".umb-editor-sub-view__content")[0];
    }

    /**
     * Adds a callback for the given tab and tab id
     * @param {HtmlElement} tab The tab
     * @param {number} tabId Tab id
     * @param {Function} callback Callback function
     */
    function setupObservers(tab, tabId, contentApp, contentAppId, callback) {
        setupTabObserver(tab, tabId, contentApp, callback);

        if (contentApp != null && contentApp !== tab) {
            setupContentAppObserver(tab, tabId, contentApp, contentAppId);
        }
    }

    function setupTabObserver(tab, tabId, contentApp, callback) {
        var data = observedTabs[tabId];
        if (data == null) {
            var observer = createTabObserver(tab, contentApp, function observerCallback() {
                runCallbacks(tabId);
            });

            data = observedTabs[tabId] = {
                callbacks: [],
                observer: observer
            };
        }

        data.callbacks.push(callback);
    }

    function setupContentAppObserver(tab, tabId, contentApp, contentAppId) {
        var data = observedContentApps[contentAppId];
        if (data == null) {
            var observer = createContentAppObserver(tab, contentApp, function observerCallback() {
                runContentAppCallbacks(contentAppId);
            });

            data = observedContentApps[contentAppId] = {
                tabIds: [],
                observer: observer
            };
        }

        if (data.tabIds.indexOf(tabId) === -1) {
            data.tabIds.push(tabId);
        }
    }

    /**
     * Removes a callback for the given tab id
     * @param {number} tabId Tab id
     * @param {Function} callback Callback to remove
     */
    function stopObserving(tabId, contentAppId, callback) {
        removeTabCallback(tabId, callback);

        if (observedTabs[tabId] == null) {
            removeContentAppTabId(tabId, contentAppId);
        }
    }

    function removeTabCallback(tabId, callback) {
        var data = observedTabs[tabId];
        if (data != null) {
            var callbacks = data.callbacks;
            if (callbacks != null) {
                var idx = callbacks.indexOf(callback);
                if (idx > -1) {
                    callbacks.splice(idx, 1);

                    if (callbacks.length === 0) {
                        data.observer.disconnect();
                        delete observedTabs[tabId];
                    }
                }
            }
        }
    }

    function removeContentAppTabId(tabId, contentAppId) {
        var data = observedContentApps[contentAppId];
        if (data != null) {
            var tabIds = data.tabIds;
            if (tabIds != null) {
                var idx = tabIds.indexOf(tabId);
                if (idx > -1) {
                    tabIds.splice(idx, 1);

                    if (tabIds.length === 0) {
                        data.observer.disconnect();
                        delete observedContentApps[contentAppId];
                    }
                }
            }
        }
    }

    /**
     * Runs all callbacks for the given tab id
     * @param {number} tabId Tab id
     */
    function runCallbacks(tabId) {
        var data = observedTabs[tabId];
        if (data != null) {
            var callbacks = data.callbacks;
            if (callbacks != null) {
                var i = callbacks.length;
                while (i--) {
                    callbacks[i]();
                }

                // Make sure Angular picks up the changes
                $rootScope.$digest();
            }
        }
    }

    /**
     * Runs all callbacks for the given content app id
     * @param {number} contentAppId Content app id
     */
    function runContentAppCallbacks(contentAppId) {
        var data = observedContentApps[contentAppId];
        if (data != null) {
            var tabIds = data.tabIds;
            if (tabIds != null) {
                var i = tabIds.length;
                while (i--) {
                    var tabId = tabIds[i];
                    runCallbacks(tabId);
                }
            }
        }
    }

    // Public API
    this.subscribe = subscribe;
}
