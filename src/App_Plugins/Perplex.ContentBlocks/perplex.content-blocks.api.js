angular.module("umbraco").service("Perplex.ContentBlocks.Api", [
    "$http", "Perplex.Downloader",
    function ($http, PerplexDownloader) {        
        var previewApi = new PerplexDownloader("/umbraco/backoffice/api/contentblockspreviewapi/");
        var definitionApi = new PerplexDownloader("/umbraco/backoffice/api/contentblocksdefinitionapi/");
        var usageApi = new PerplexDownloader("/umbraco/backoffice/api/contentblocksusageapi/");
        var presetApi = new PerplexDownloader("/umbraco/backoffice/api/contentblockspresetapi/");

        this.getPreview = function (pageId) {
            return previewApi.post("GetPreview", { pageId: pageId });
        }

        this.getAllDefinitions = function () {
            return definitionApi.get("GetAllDefinitions");
        }

        this.getDefinitionsForPage = function (documentType, culture) {
            return definitionApi.get("GetDefinitionsForPage?documentType=" + documentType + "&culture=" + culture);
        }

        this.getAllCategories = function () {
            return definitionApi.get("GetAllCategories");
        }

        this.getAllContentBlockUses = function (websiteId, culture) {
            return usageApi.get("GetAllContentBlockUses?websiteId=" + websiteId + "&culture=" + culture);
        }

        this.getPresetForPage = function (documentType, culture) {
            return presetApi.get("GetPresetForPage?documentType=" + documentType + "&culture=" + culture);
        }

        this.getWebsiteId = function (pageId) {
            return usageApi.get("GetWebsiteId?pageId=" + pageId);
        }
    }
]);
