angular.module("perplexContentBlocks").service("contentBlocksPropertyScaffoldCache", [
    "contentTypeResource",
    "dataTypeResource",
    function contentBlocksPropertyScaffoldCache(contentTypeResource, dataTypeResource) {
        /**
         * Cache of promises in progress and/or completed by data type id
         */
        var scaffoldsById = {};

        /**
         * Cache of promises of data type key => data type id.
         */
        var dataTypeKeyToId = {};

        function getByKey(key) {
            if (dataTypeKeyToId[key] == null) {
                dataTypeKeyToId[key] = dataTypeResource.getById(key).then(function (dataType) {
                    if (dataType == null) {
                        throw new Error('No data type found with key "' + key + '"');
                    }

                    return dataType.id;
                });
            }

            return dataTypeKeyToId[key].then(
                function (id) {
                    return getById(id);
                },
                function () {
                    throw new Error('No data type found with key "' + key + '"');
                }
            );
        }

        function getById(id) {
            if (scaffoldsById[id] == null) {
                scaffoldsById[id] = contentTypeResource.getPropertyTypeScaffold(id);
            }

            return scaffoldsById[id].then(
                function (value) {
                    return angular.copy(value);
                },
                function () {
                    throw new Error("No data type found with id " + id);
                }
            );
        }

        function getScaffold(idOrKey) {
            if (typeof idOrKey === "number") {
                return getById(idOrKey);
            } else if (typeof idOrKey === "string") {
                return getByKey(idOrKey);
            } else {
                throw new Error("getScaffold: supply either a number or string (guid).");
            }
        }

        /**
         * Clears the cached scaffold for the data type with the given id.
         * @param {number} id Data type id.
         */
        function clearCache(id) {
            delete scaffoldsById[id];
        }

        this.getScaffold = getScaffold;
        this.getScaffoldById = getById;
        this.getScaffoldByKey = getByKey;
        this.clearCache = clearCache;
    },
]);

// Ensure scaffold cache is cleared whenever a data type is saved.
angular.module("perplexContentBlocks").decorator("dataTypeResource", [
    "$delegate",
    "$injector",
    function ($delegate, $injector) {
        var saveFn = $delegate.save;

        $delegate.save = function () {
            return saveFn.apply(this, arguments).then(function (dataType) {
                updateScaffoldCache(dataType);
                return dataType;
            });
        };

        function updateScaffoldCache(dataType) {
            if (dataType == null) return;

            var scaffoldCache = $injector.get("contentBlocksPropertyScaffoldCache");
            if (scaffoldCache == null) return;

            scaffoldCache.clearCache(dataType.id);
        }

        return $delegate;
    },
]);
