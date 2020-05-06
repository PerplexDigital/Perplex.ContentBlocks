angular.module("perplexContentBlocks").service("contentBlocksPropertyScaffoldCache", [
    "contentTypeResource",
    "dataTypeResource",
    function contentBlocksPropertyScaffoldCache(contentTypeResource, dataTypeResource) {
        /**
         * Cache of promises in progress and/or completed by id
         * Note the id can be either an integer or guid.
         */
        var scaffoldsById = {};

        /**
         * Cache of promises of dataType key => dataType id.
         */
        var dataTypeKeyToId = {};

        function getByKey(key) {            
            if (dataTypeKeyToId[key] == null) {
                dataTypeKeyToId[key] = dataTypeResource.getAll().then(function (dataTypes) {
                    var dataType = _.find(dataTypes, function (dataType) {
                        return dataType.key === key;
                    });
                    if (dataType == null) {
                        throw new Error("No data type found with key \"" + key + "\"");
                    } 

                    return dataType.id;
                });
            }

            return dataTypeKeyToId[key].then(function (id) {
                return getById(id);
            }, function () {
                throw new Error("No data type found with key \"" + key + "\"");
            });
        }

        function getById(id) {
            if (scaffoldsById[id] == null) {
                scaffoldsById[id] = contentTypeResource.getPropertyTypeScaffold(id);
            }

            return scaffoldsById[id].then(function (value) {
                return angular.copy(value);
            }, function () {
                throw new Error("No data type found with id \"" + id + "\"");
            });
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

        this.getScaffold = getScaffold;
        this.getScaffoldById = getById;
        this.getScaffoldByKey = getByKey;
    }
]);
