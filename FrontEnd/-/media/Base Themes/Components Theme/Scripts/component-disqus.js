/**
 * Component Disqus
 * @module Disqus
 * @param  {jQuery} $ Instance of jQuery
 * @param  {Document} document Instance of Document
 * @return {Object} List of Disqus methods
 */
XA.component.disqus = (function($, document) {
    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Archive
     * */
    var api = {};
    /**
     * initDisqus method create script element  for discuss component
     * @memberOf module:Disqus
     * @method
     * @param {Object} prop option of disqus component
     * @alias module:Disqus.initDisqus
     * @private
     */
    function initDisqus(prop) {
        var dsq = document.createElement("script");
        dsq.type = "text/javascript";
        dsq.async = true;
        dsq.src = "//" + prop.disqus_shortname + ".disqus.com/embed.js";
        (
            document.getElementsByTagName("head")[0] ||
            document.getElementsByTagName("body")[0]
        ).appendChild(dsq);
    }
    /**
     * initInstance method set up disqus_config and call
     * [".initDisqus"]{@link module:Disqus.initDisqus}  method.
     * @memberOf module:Disqus
     * @method
     * @param {jQuery} component Root DOM element of archive component wrapped by jQuery
     * @param {Object} prop option of disqus component
     * @alias module:Disqus.initInstance
     */
    api.initInstance = function(component, prop) {
        window.disqus_config = function() {
            this.page.url = prop.disqus_url;
            this.page.identifier = prop.disqus_identifier;
            this.page.title = prop.disqus_title;
            this.page.category_id = prop.disqus_category_id;
        };

        if (component.find("#disqus_thread").length > 0) {
            initDisqus(prop);
        }
    };
    /**
     * init method calls in a loop for each
     * disqus component on a page and run Disqus's
     * [".initInstance"]{@link module:Disqus.initInstance}  method.
     * @memberOf module:Disqus
     * @alias module:Disqus.init
     */
    api.init = function() {
        var disqus = $(".disqus:not(.initialized)");
        disqus.each(function() {
            var properties = $(this).data("properties");
            api.initInstance($(this), properties);
            $(this).addClass("initialized");
        });
    };

    return api;
})(jQuery, document);

XA.register("disqus", XA.component.disqus);
