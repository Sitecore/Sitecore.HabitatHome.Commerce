/**
 * Component Galleria
 * @module Galleria
 * @param  {jQuery} $ Instance of jQuery
 * @return {Object} List of Galleria component methods
 */
XA.component.galleria = (function($) {
    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Galleria
     * */
    /* global Galleria:false */
    var api = {};
    /**
     * Call "Galleria.loadTheme" and  "Galleria.run"
     * from [galleria.io]{@link https://galleria.io/}. More about this methods you can
     * read [here]{@link https://docs.galleria.io/article/131-static-methods}
     * @memberOf module:Galleria
     * @method
     * @param {jQuery} component Root DOM element of galleria component wrapped by jQuery
     * @param {Object} prop Properties setted in data attribute
     *  of galleria component
     * @alias module:Galleria.initInstance
     */
    api.initInstance = function(component, prop) {
        var id = component.find(".gallery-inner").attr("id");
        Galleria.loadTheme(prop.theme);
        Galleria.run("#" + id, prop);
    };
    /**
     * Find all not initialized yet
     * Gallery component and in a loop for each of them
     * run
     * ["initInstance"]{@link module:Galleria.initInstance}
     * method.
     * @memberOf module:Galleria
     * @alias module:Galleria.init
     */
    api.init = function() {
        var gallery = $(".gallery:not(.initialized)");

        gallery.each(function() {
            var properties = $(this).data("properties");
            api.initInstance($(this), properties);
            $(this).addClass("initialized");
        });
    };

    return api;
})(jQuery, document);

XA.register("galleria", XA.component.galleria);
