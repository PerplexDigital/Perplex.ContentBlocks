angular.module("umbraco").factory("Perplex.Downloader", [
    "Perplex.Download.Service",
    function(service) {
        function PerplexDownloader(rootUrl) {
            this.rootUrl = rootUrl;
        }

        PerplexDownloader.prototype.get = function(path, timeout) {
            return service.get(this.rootUrl + path, timeout);
        };

        PerplexDownloader.prototype.post = function(path, data, timeout) {
            return service.post(this.rootUrl + path, data, timeout);
        };

        /**
         * @param {string} path Pad naar API methode, URL naar controller zelf
         * @param {string} method API Method (GET / POST)
         * @param {*} data Data voor POST request
         * @param {function()} beforeFn Functie die voor de API call moet worden uitgevoerd
         * @param {function(*,*,*)} errorFn Functie die bij een fout wordt aangeroepen
         * @param {function()} afterFn Functie die na de API call moet worden uitgevoerd
         */
        PerplexDownloader.prototype.download = function(path, method, data, beforeFn, errorFn, afterFn) {
            if (isFn(beforeFn)) {
                beforeFn();
            }

            service
                .arraybuffer(this.rootUrl + path, method, data)
                .then(service.downloadWithBrowser, errorFn || service.handleError)
                .always(afterFn);
        };

        function isFn(fn) {
            return typeof fn === "function";
        }

        return PerplexDownloader;
    }
]);
