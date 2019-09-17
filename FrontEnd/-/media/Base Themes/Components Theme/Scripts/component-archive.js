/**
 * Component Archive
 * @module Archive
 * @param  {jQuery} $ Instance of jQuery
 * @return {Object} List of archive methods
 */
XA.component.archive = (function($) {
    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Archive
     * */
    var api = {};
    /**
     * toggleClick method toggle childs visibility
     * for archive items
     * @memberOf module:Archive
     * @param {eventObject} args
     * @private
     */
    var toggleClick = function(args) {
        var groupHeader = $(args.target);
        groupHeader.siblings("ul").toggle();
        groupHeader.toggleClass("opened");
    };
    /**
     * initInstance method bind toggling for
     * an arhive element
     * @memberOf module:Archive
     * @method
     * @param {jQuery} component Root DOM element of archive component wrapped by jQuery
     * @alias module:Archive.initInstance
     */
    api.initInstance = function(component) {
        var toggles = component.find(".group-header");
        toggles.on("click", toggleClick);
    };

    /**
     * init method calls in a loop for each
     * archive component on a page and run Archive's
     * ["initInstance"]{@link module:Archive.api.initInstance}  method.
     * @memberOf module:Archive
     * @alias module:Archive.init
     */
    api.init = function() {
        var archives = $(".sxa-archive:not(.initialized)"),
            archive;

        for (var i = 0, l = archives.length; i < l; i++) {
            archive = $(archives[i]);
            api.initInstance(archive);
            archive.addClass("initialized");
        }
    };

    return api;
})(jQuery, _);

XA.register("archive", XA.component.archive);
