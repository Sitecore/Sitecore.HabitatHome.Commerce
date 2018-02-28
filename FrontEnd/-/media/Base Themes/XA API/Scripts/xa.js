var XA = XA || (function ($, document) {
    var api = {}, onPreInitHandlers, onPostInitHandlers, modules = {};
    onPreInitHandlers = new Array();
    onPostInitHandlers = new Array();
    api.register = function (name, api, init) {
        modules[name] = {
            name: name,
            api: api,
            init: init || api.init || (function () { })
        };
    };
    api.registerOnPreInitHandler = function (handler) { onPreInitHandlers.push(handler); };
    api.registerOnPostInitHandler = function (handler) { onPostInitHandlers.push(handler); };
    var initScheduled = false;
    api.init = function () {
        if (!initScheduled) {
            initScheduled = true;
            XA.ready(function () {
                try {
                    for (var name in modules)
                        if (modules.hasOwnProperty(name)) {
                            $xa.each(onPreInitHandlers, function (i, h) { h.process(name, modules[name]); });
                            modules[name].init();
                            $xa.each(onPostInitHandlers, function (i, h) { h.process(name, modules[name]); });
                        }
                }
                finally {
                    initScheduled = false;
                }
            });
        }
    };
    api.ready = function (fn) {
        $(document).ready(fn);
    };
    api.component = {};
    api.connector = {};
    api.cookies = {
        createCookie: function (name, value, days) {
            if (days) {
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                var expires = "; expires=" + date.toUTCString();
            }
            else {
                expires = "";
            }
            document.cookie = name + "=" + value + expires + "; path=/";
        },
        readCookie: function (name) {
            var nameEQ = name + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') {
                    c = c.substring(1, c.length);
                }
                if (c.indexOf(nameEQ) == 0) {
                    return c.substring(nameEQ.length, c.length);
                }
            }
            return null;
        },
        removeCookieWarning: function () {
            var cookieWarning = $xa(".privacy-warning");
            cookieWarning.remove();
        }
    };
    return api;
})($, document);
XA.init();
