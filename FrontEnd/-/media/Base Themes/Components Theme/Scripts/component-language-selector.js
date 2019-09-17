/**
 * Component Language Selector
 * @module LanguageSelector
 * @param  {jQuery} $ Instance of jQuery
 * @return {Object} List of language selector methods methods
 */
XA.component.languageSelector = (function($) {
    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:LanguageSelector
     * */
    var api = {};
    /**
     * readDataAttributes function that return
     * name of a language flag in format
     * 'flags-'+language name
     * @memberOf module:LanguageSelector
     * @param {jQuery} item
     * @private
     */
    function readDataAttributes(item) {
        var country = item.data("country-code");

        return "flags-" + country;
    }
    /**
     * initLanguageSelector setted up flag classes for laguage selector items
     * and bind 'click' event to drop down item that will toggle it visibility
     * @memberOf module:LanguageSelector
     * @param {jQuery} instance Root DOM element of language selector
     * component
     */
    function initLanguageSelector(instance) {
        var el = $(instance),
            header = el.find(".language-selector-select-item"),
            dropDownList = el.find(".language-selector-item-container"),
            dropDownItem = dropDownList.find(".language-selector-item");

        var className = readDataAttributes(header);
        header.find(">a").addClass(className);

        dropDownList.find(".language-selector-item").each(function() {
            className = readDataAttributes($(this));
            $(this)
                .find(">a")
                .addClass(className);
        });

        header.on("click", function() {
            dropDownList.slideToggle();
        });

        dropDownItem.on("click", function() {
            var url = $(this)
                .find("a")
                .attr("href");

            window.location.href = url;
        });
    }
    /**
     * initInstance method calls
     * [".initLanguageSelector"]{@link module:LanguageSelector.initLanguageSelector}  method.
     * @memberOf module:LanguageSelector
     * @method
     * @param {jQuery} component Root DOM element of language selector component
     * @alias module:LanguageSelector.initInstance
     */
    api.initInstance = function(component) {
        initLanguageSelector(component);
    };

    /**
     * init method calls in a loop for each
     * language selector component on a page and run LanguageSelector's
     * [".initInstance"]{@link module:LanguageSelector.api.initInstance}  method.
     * @memberOf module:LanguageSelector
     * @alias module:LanguageSelector.init
     */
    api.init = function() {
        var languageSelector = $(".language-selector:not(.initialized)");

        languageSelector.each(function() {
            api.initInstance(this);
            $(this).addClass("initialized");
        });
    };

    return api;
})(jQuery, document);

XA.register("language-selector", XA.component.languageSelector);
