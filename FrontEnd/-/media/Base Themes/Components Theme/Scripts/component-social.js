/**
 * Component Social
 * @module Social
 * @param  {jQuery} $ Instance of jQuery
 * @param  {document} document document object
 * @return {Object} List of Social component methods
 */
XA.component.social = (function($, document) {
    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Social
     * @
     * */
    var api = {};
    api.shareProperties;
    /**
     * Create connector for facebook
     * initInstance method of a Social component
     * @memberOf module:Social
     * @method
     * @alias module:Social.initFacebook
     */
    api.initFacebook = function() {
        (function(d, s, id) {
            var js,
                fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) {
                return;
            }
            js = d.createElement(s);
            js.id = id;
            js.src = "//connect.facebook.net/en_US/all.js#xfbml=1";
            fjs.parentNode.insertBefore(js, fjs);
        })(document, "script", "facebook-jssdk");
    };
    /**
     * Create connector for facebook
     * initInstance method of a Social component
     * @memberOf module:Social
     * @param {Array} properties properties for external scripts
     * @method
     * @alias module:Social.initFacebook
     */
    var attachExternalScript = function(properties) {
        var component = $(".sharethis"),
            shareThisExternal;
        if (window.stLight === undefined) {
            shareThisExternal = document.createElement("script");
            shareThisExternal.type = "text/javascript";
            shareThisExternal.src = "//w.sharethis.com/button/buttons.js";
            $(window).on("load", function() {
                $(properties).each(function() {
                    try {
                        window.stLight.options(this);
                    } catch (e) {}
                });
            });
            $(component[0]).append(shareThisExternal);
        }
    };
    /**
     * initInstance method of a Social component
     * @memberOf module:Social
     * @method
     * @param {jQuery} component Root DOM element of Social component wrapped by jQuery
     * @param {Object} prop Social component properties
     * @alias module:Social.initInstance
     */
    api.initInstance = function(component, prop) {
        api.shareProperties.push(prop);
    };
    /**
     * init method calls in a loop for each
     * social component on a page and run
     * ["initInstance"]{@link module:Snippet.api.initInstance} methods.
     * @memberOf module:Social
     * @alias module:Social.init
     */
    api.init = function() {
        var shareThis = $(".sharethis:not(.initialized)");
        api.shareProperties = [];
        shareThis.each(function() {
            var properties = $(this).data("properties");
            api.initInstance($(this), properties);
            $(this).addClass("initialized");
        });
        attachExternalScript(api.shareProperties);
    };
    return api;
})(jQuery, document);

XA.register("social", XA.component.social);
