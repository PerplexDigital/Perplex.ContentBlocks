angular.module("umbraco").service("perplexRenderPropertyService", [
    "contentTypeResource",
    "dataTypeResource",
    "$q",
    function(contentTypeResource, dataTypeResource, $q) {
        /**
         * Cache of promises in progress and/or completed
         * by alias and name
         */
        var cacheByAlias = {}; // alias => name => promise

        /**
         * Cache of promises in progress and/or completed by guid
         */
        var cacheByGuid = {};

        /**
         * Cache of promises in progress and/or completed by id
         */
        var cacheById = {};

        /**
         * Returns the scaffold for a property editor with the given alias.
         * If a name is given, will only return the scaffold for the datatype
         * if it also has that name (in addition to the alias).
         * @param {any} alias Alias of the property editor
         * @param {string} [name] Name of the datatype
         * @returns {Promise}
         */
        function getPropertyTypeScaffoldByAlias(alias, name) {
            if (cacheByAlias[alias] == null) {
                cacheByAlias[alias] = {};
            }

            var cached = cacheByAlias[alias][name];
            if (cached == null) {
                cached = cacheByAlias[alias][name] = dataTypeResource.getAll().then(function (response) {
                    // There can be multiple datatypes with the same property editor alias
                    // (e.g., multiple custom listviews which all have Umbraco.ListView as alias)
                    var dataTypes = _.filter(response, { alias: alias });
                    if (dataTypes.length > 0) {
                        return getScaffoldFromDataTypes(dataTypes);
                    } else {
                        // There is also something called 'grouped datatypes' which contains groups like 'lists',
                        // for example the Umbraco.ListView. Somehow, the built-in datatypes (e.g. List view - Content) are hidden here
                        // and are not returned by dataTypeResource.getAll() for some reason (???).
                        // Anyway, we will look there too when we cannot find the alias in the getAll().
                        return dataTypeResource.getGroupedDataTypes().then(function (response) {
                            // This response contains keys of categories which point to an Array of datatypes
                            // First join them together in a big Array
                            var allTypes = _.flatten(_.values(response));
                            var dataTypes = _.filter(allTypes, { alias: alias });

                            if (dataTypes.length === 0) {
                                throw new Error("No property editor with alias \"" + alias + "\" was found.");
                            }

                            return getScaffoldFromDataTypes(dataTypes);
                        });
                    }

                    function getScaffoldFromDataTypes(dataTypes) {
                        // Optionally filter by name
                        var targetTypes = name != null ? _.filter(dataTypes, { name: name }) : dataTypes;

                        // Take the first remaining element
                        var dataType = targetTypes[0];

                        if (dataType == null) {
                            throw new Error("None of the datatypes with alias \"" + alias + "\" have a name equal to \"" + name + "\"");
                        }

                        return getPropertyTypeScaffoldById(dataType.id);
                    }
                });
            }

            // Return a promise that is resolved with a
            // copy of the original promise return value
            return cached.then(function (value) {
                return angular.copy(value);
            });
        }

        function getPropertyTypeScaffoldByGuid(guid) 
        {
            var cached = cacheByGuid[guid];
            if (cached == null) {
                cached = cacheByGuid[guid] = dataTypeResource.getAll().then(function (response) {
                    var dataType = _.find(response, { key: guid });
                    if (dataType !== undefined) {
                        return getPropertyTypeScaffoldById(dataType.id);
                    } else {
                        throw new Error("No data type found with guid \"" + guid + "\"");
                    }
                });
            }

            // Return a promise that is resolved with a
            // copy of the original promise return value
            return cached.then(function (value) {
                return angular.copy(value);
            });
        }

        function getPropertyTypeScaffoldById(id) {
            if (cacheById[id] == null) {
                cacheById[id] = contentTypeResource.getPropertyTypeScaffold(id);
            }

            return cacheById[id].then(function (value) {
                return angular.copy(value);
            }, function(e) {
                throw new Error("No property editor with id \"" + id + "\" was found.");
            });
        }

        this.getPropertyTypeScaffoldByAlias = getPropertyTypeScaffoldByAlias;
        this.getPropertyTypeScaffoldById = getPropertyTypeScaffoldById;
        this.getPropertyTypeScaffoldByGuid = getPropertyTypeScaffoldByGuid;
    }
]);
