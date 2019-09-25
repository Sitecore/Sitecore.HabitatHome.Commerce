/**
 * Component Flash
 * @module Flash
 * @param  {jQuery} $ Instance of jQuery
 * @return {Object} List of flash methods
 */
XA.component.flash = (function($) {
    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Flash
     * */
    var api = {};
    /**
     * Set size of embeded element
     * @memberOf module:Flash
     * @method
     * @param {jQuery.<Element>} object Embed element
     * @private
     */
    function setSize(object) {
        var oldHeight = object.attr("height");
        var oldWidth = object.attr("width");
        var newWidth = object.width();
        var newHeight = (oldHeight * newWidth) / oldWidth;
        object.height(newHeight);
    }
    /**
     * Initialize flash component
     * @memberOf module:Flash
     * @method
     * @param {jQuery} component Root DOM element of flash component wrapped by jQuery
     * @param {Object} properties Properties setted in data attribute
     * @private
     */
    function initFlash(component, properties) {
        var content = component.find(".component-content > div");
        content.flash(properties);
    }
    /**
     * Call ["setSize"]{@link module:Flash.setSize} method
     * and bind it to resize event
     * @memberOf module:Flash
     * @method
     * @param {jQuery} component Root DOM element of flash component wrapped by jQuery
     * @param {Object} prop Properties setted in data attribute
     * @private
     */
    function attachEvents(component) {
        $(document).ready(function() {
            var object = component.find("embed");
            object.css("width", "100%");
            setSize(object);

            $(window).resize(function() {
                setSize(object);
            });
        });
    }

    /**
     * Call
     * ["initFlash"]{@link  module:Flash.initFlash} and
     * ["attachEvents"]{@link  module:Flash.attachEvents}
     * methods
     * @memberOf module:Flash
     * @method
     * @param {jQuery} component Root DOM element of flash component wrapped by jQuery
     * @param {Object} prop Properties setted in data attribute
     *  of flash component
     * @alias module:Flash.initInstance
     */
    api.initInstance = function(component, prop) {
        initFlash(component, prop);
        attachEvents(component);
    };
    /**
     * Find all not initialized yet
     * Flash component and in a loop for each of them
     * run Flash's 
     * ["initInstance"]{@link module:Flash.initInstance}
     * method.
     * @memberOf module:Flash
     * @alias module:Flash.init
     */
    api.init = function() {
        var flash = $(".flash:not(.initialized)");
        if (flash.length > 0) {
            flash.each(function() {
                var properties = $(this).data("properties");
                api.initInstance($(this), properties);
                $(this).addClass("initialized");
            });
        }
    };

    return api;
})(jQuery, document);

XA.register("flash", XA.component.flash);
