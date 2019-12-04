/**
 * Component Snippet
 * @module Snippet
 * @param  {jQuery} $ Instance of jQuery
 * @return {Object} List of Snippet methods
 */
XA.component.snippet = (function($) {
     /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Snippet
     * @
     * */
    var api = {},
        instance;
     /**
     * initInstance method of a Snippet element
     * @memberOf module:Snippet
     * @method
     * @param {jQuery} component Root DOM element of Snippet component wrapped by jQuery
     * @param {jQuery} snippetModule Snippet inner DOM element of Snippet component wrapped by jQuery
     * @alias module:Snippet.initInstance
     */
    api.initInstance = function(component, snippetModule) {};
    /**
     * init method calls in a loop for each
     * snippet component on a page and run Snippet's
     * ["initInstance"]{@link module:Snippet.api.initInstance} methods.
     * @memberOf module:Snippet
     * @alias module:Snippet.init
     */
    api.init = function() {
        var $snippets = $(".snippet:not(.initialized)");
        $snippets.each(function() {
            var $snippetModule = $(this).find(".snippet-inner");
            instance = $(this);
            api.initInstance(instance, $snippetModule);
            $(this).addClass("initialized");
        });
    };

    return api;
})(jQuery);

XA.register("snippet", XA.component.snippet);
