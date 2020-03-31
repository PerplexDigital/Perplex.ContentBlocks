angular.module("umbraco").service("Perplex.ContentBlocks.CopyPaste.Service", ["Perplex.ContentBlocks.Service", "Perplex.LocalStorage.Service",    
    function (service, localStorageService) {
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

            // The actual onChange is done via the localStorageService implementation
            // with a single registration.
            if (!onChangeRegistered) {
                localStorageService.onChange(function () {
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
            var copies = service.copyAll(data);
            localStorageService.save(STORAGE_KEY, copies);
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
    
        function hasAny() {
            return localStorageService.contains(STORAGE_KEY);
        }

        function getAll() {
            return localStorageService.get(STORAGE_KEY);
        }

        function clear() {
            localStorageService.remove(STORAGE_KEY);
            triggerOnChange(null);
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

        this.copyAll = copyAll;
        this.copyBlock = copyBlock;
        this.copyBlocks = copyBlocks;
        this.copyHeader = copyHeader;
        this.hasAny = hasAny;        
        this.getAll = getAll;
        this.pasteAll = pasteAll;
        this.pasteHeader = pasteHeader;
        this.pasteBlocks = pasteBlocks;
        this.clear = clear;
        this.onChange = onChange;
    }
]);
