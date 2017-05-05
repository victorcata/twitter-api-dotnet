var app = app || {};

/**
 * Global functions
 */
(function (global) { 
    /**
     * Ajax calls
     * @param {string} url URL to recover the data
     * @param {function} callback Function to execute once the data is recovered
     */
    global.get = function (url, callback, cbError) {
        let xhr = new XMLHttpRequest();

        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    callback(xhr.responseText);
                } else {
                    if (cbError && typeof cbError === "function") {
                        cbError(xhr.statusText);
                    }
                }
            }
        };

        xhr.open("GET", url, true);
        xhr.send();
    }

    /**
     * Ajax calls
     * @param {string} url URL to recover the data
     * @param {function} callback Function to execute once the data is recovered
     */
    global.getJSON = function (url, callback, cbError) {
        let xhr = new XMLHttpRequest();

        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    callback(JSON.parse(xhr.responseText));
                } else {
                    if (cbError && typeof cbError === "function") {
                        cbError(xhr.statusText);
                    }
                }
            }
        };

        xhr.open("GET", url, true);
        xhr.send();
    }
})(window);

/**
 * Twitter feed update
 * https://dev.twitter.com/rest/public/search
 */
(function (global) {

    app.Twitter = (function(){
        var _defaults = {
            count: config.twitter.count || 12,
            hashtag = config.twitter.hashtag || "<DEFAULT_HASHTAG>"
        };

        /**
         * Gets new tweets every X seconds
         */
        function _refreshTweets() {
            if (document.getElementById("tweets") === null) return;

            global.get(`/Components/Tweets?count=${defaults.count}&hashtag=${defaults.hashtag}`, function (output) {
                document.getElementById("tweets").outerHTML = output;
            });
        }
        
        return {
            refresh: _refreshTweets
        }
    })();
})(window);