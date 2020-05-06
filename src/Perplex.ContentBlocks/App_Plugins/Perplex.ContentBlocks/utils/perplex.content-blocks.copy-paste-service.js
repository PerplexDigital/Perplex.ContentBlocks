angular.module("perplexContentBlocks").service("contentBlocksCopyPasteService", ["contentBlocksUtils",
    contentBlocksCopyPasteService
]);

function contentBlocksCopyPasteService(utils) {
    var localStorage = window.localStorage;
    if (typeof localStorage !== "object") {
        throw new Error("Requires window.localStorage object");
    }

    var STORAGE_KEY = "Perplex.ContentBlocks.CopyPaste";

    var onChangeCallbacks = [];

    var onChangeRegistered = false;

    function onChange(callback) {
        if (typeof callback !== "function") {
            throw new Error("'callback' should be a function");
        }

        onChangeCallbacks.push(callback);

        // Immediately invoke with current data
        callback(getAll());

        if (!onChangeRegistered) {
            localStorageOnChange(function () {
                var data = getAll();
                triggerOnChange(data);
            }, STORAGE_KEY);

            onChangeRegistered = true;
        }
    }

    function triggerOnChange(data) {
        for (var i = 0; i < onChangeCallbacks.length; i++) {
            var callback = onChangeCallbacks[i];
            if (typeof callback === "function") {
                callback(data);
            }
        }
    }

    function copyAll(data) {
        var copies = utils.copyAll(data);
        save(copies);
        triggerOnChange(data);
    }

    function copyHeader(header) {
        copyAll({ header: header });
    }

    function copyBlock(block) {
        copyAll({ blocks: [block] });
    }

    function copyBlocks(blocks) {
        copyAll({ blocks: blocks });
    }

    function pasteAll(callback) {
        if (typeof callback !== "function") {
            throw new Error("'callback' should be a function and is passed parameters header, blocks")
        }

        var data = getAll();
        if (data != null && typeof callback === "function") {
            callback(data.header, data.blocks);
        }
    }

    function pasteHeader(callback) {
        var data = getAll();
        if (data != null && data.header != null && typeof callback === "function") {
            callback(data.header);
        }
    }

    function pasteBlocks(callback) {
        var data = getAll();
        if (data != null && Array.isArray(data.blocks) && typeof callback === "function") {
            callback(data.blocks);
        }
    }

    function save(value) {
        if (!value) {
            return;
        }

        var json = JSON.stringify(value);
        localStorage.setItem(STORAGE_KEY, json);
    }

    function remove(key) {
        localStorage.removeItem(key);
    }

    function localStorageOnChange(callback) {
        if (typeof callback !== "function") {
            throw new Error("callback should be a function.");
        }

        window.addEventListener("storage", function (e) {
            if (e.key !== STORAGE_KEY) {
                return;
            }

            callback({
                key: e.key,
                oldValue: e.oldValue,
                newValue: e.newValue
            });
        });
    }

    function getAll() {
        try {
            var value = localStorage.getItem(STORAGE_KEY);
            if (value) {
                return JSON.parse(value);
            }
        } catch (e) {
            return null;
        }
    }

    function clear() {
        remove(STORAGE_KEY);
        triggerOnChange(null);
    }

    this.copyAll = copyAll;
    this.copyBlock = copyBlock;
    this.copyBlocks = copyBlocks;
    this.copyHeader = copyHeader;
    this.getAll = getAll;
    this.pasteAll = pasteAll;
    this.pasteHeader = pasteHeader;
    this.pasteBlocks = pasteBlocks;
    this.clear = clear;
    this.onChange = onChange;
}
