angular.module("umbraco").service("Perplex.Download.Service", [
    "$http",
    "notificationsService",
    function($http, notificationsService) {
        /**
         * Biedt het bestand dat in de response zit als download aan in de browser
         * @param {*} response $http response object van het type arraybuffer
         */
        function downloadWithBrowser(response) {
            var headers = response.headers();
            var contentDisposition = headers["content-disposition"];
            var contentType = headers["content-type"];

            var filenameMatch = contentDisposition.match(/filename=(")?(.*)\1/);
            if (filenameMatch.length != 3) {
                return;
            }
            var filename = filenameMatch[2];

            var blob = new Blob([response.data], { type: contentType });

            // Internet Explorer doet het natuurlijk weer anders
            // maar op zich wel wat makkelijker dan de rest
            if (typeof window.navigator.msSaveOrOpenBlob === "function") {
                window.navigator.msSaveOrOpenBlob(blob, filename);
            } else {
                var url = URL.createObjectURL(blob);
                var a = document.createElement("a");
                a.href = url;
                a.download = filename;
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
                URL.revokeObjectURL(url);
            }
        }

        function get(url, timeout) {
            return $http.get(url, { timeout: timeout && timeout.promise });
        }

        function post(url, data, timeout) {
            return $http.post(url, data, { timeout: timeout && timeout.promise });
        }

        function arraybuffer(url, method, data) {
            var options = {
                url: url,
                method: method,
                responseType: "arraybuffer"
            };

            if (data != null) {
                options.data = data;
            }

            return $http(options);
        }

        function handleError(data, status, headers, config) {
            var notification = {
                type: "error",
                headline: data.data.Message || data.status
            };
            notificationsService.add(notification);
        }

        this.downloadWithBrowser = downloadWithBrowser;
        this.post = post;
        this.get = get;
        this.arraybuffer = arraybuffer;
        this.handleError = handleError;
    }
]);
