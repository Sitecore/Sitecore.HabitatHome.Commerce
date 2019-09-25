/**
 * Component facebook
 * @module Facebook
 * @param  {jQuery} $ Instance of jQuery
 * @param  {Document} document Instance of Document
 * @return {Object} List of Facebook methods
 */
XA.component.facebook = (function($, document) {
    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Facebook
     */
    var api = {};
    /**
     * initInstance method set up facebook properties and
     * add facebook script
     * @memberOf module:Facebook
     * @method
     * @alias module:Facebook.initInstance
     */
    api.initInstance = function() {
        (function(d, s, id) {
            var js,
                script = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) {
                return;
            }
            js = d.createElement(s);
            js.id = id;
            js.src = "//connect.facebook.net/en_US/sdk.js#xfbml=1&version=v2.7";
            script.insertBefore(js, script.firstChild);
        })(document, "script", "facebook-jssdk");
    };
    /**
     * init method calls in a loop for each
     * facebook component on a page and run Facebook's
     * [".initInstance"]{@link module:Facebook.initInstance}  method.
     * @memberOf module:Facebook
     * @alias module:Facebook.init
     */
    api.init = function() {
        var facebook = $(".fb-comments:not(.initialized)");

        facebook.each(function() {
            api.initInstance();
            $(this).addClass("initialized");
        });
    };

    return api;
})(jQuery, document);

XA.register("facebook", XA.component.facebook);
