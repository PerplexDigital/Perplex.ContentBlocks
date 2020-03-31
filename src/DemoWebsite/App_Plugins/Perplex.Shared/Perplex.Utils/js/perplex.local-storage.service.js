angular.module("umbraco").service("Perplex.LocalStorage.Service", [function () {
    function get(key) {
        try {
            var value = localStorage.getItem(key);
            if (value) {
                return JSON.parse(value);
            }
        } catch (e) {
            return null;
        }
    }

    function save(key, value) {
        if (!value) {
            return;
        }

        var json = JSON.stringify(value);
        localStorage.setItem(key, json);
    }

    function contains(key) {
        return localStorage.getItem(key) !== null;
    }

    function remove(key) {
        localStorage.removeItem(key);
    }

    function onChange(callback, key) {
        if (typeof callback !== "function") {
            throw new Error("callback should be a function.");
        }

        window.addEventListener("storage", function (e) {
            if (key != null && e.key !== key) {
                return;
            }

            callback({
                key: e.key,
                oldValue: e.oldValue,
                newValue: e.newValue
            });
        });
    }

    this.get = get;
    this.save = save;
    this.contains = contains;
    this.remove = remove;
    this.onChange = onChange;
}]);