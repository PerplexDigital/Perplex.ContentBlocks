angular.module("perplexContentBlocks").service("contentBlocksApi", ["$http",
    contentBlocksApi
]);

function contentBlocksApi($http) {
    var previewApiRoot = "/umbraco/backoffice/api/contentblockspreviewapi/";
    var definitionApiRoot = "/umbraco/backoffice/api/contentblocksdefinitionapi/";
    var presetApiRoot = "/umbraco/backoffice/api/contentblockspresetapi/";

    this.getPreview = function (pageId) {
        return $http.post(previewApiRoot + "GetPreview", { pageId: pageId });
    }

    this.getAllDefinitions = function () {
        return $http.get(definitionApiRoot + "GetAllDefinitions");
    }

    this.getDefinitionsForPage = function (documentType, culture) {
        return $http.get(definitionApiRoot + "GetDefinitionsForPage?documentType=" + documentType + "&culture=" + culture);
    }

    this.getAllCategories = function () {
        return $http.get(definitionApiRoot + "GetAllCategories");
    }

    this.getPresetForPage = function (documentType, culture) {
        return $http.get(presetApiRoot + "GetPresetForPage?documentType=" + documentType + "&culture=" + culture);
    }
}
