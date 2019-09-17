/**
 * Component Breadcrumb
 * @module Breadcrumb
 * @param  {jQuery} $ Instance of jQuery
 * @return {Object} List of archive methods
 */
XA.component.breadcrumb = (function($) {
    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Breadcrumb
     * */
    var api = {};
    /**
     * This class used by [Breadcrumb]{@link module:Breadcrumb} module
     * @class BreadcrumbManager
     * @memberOf module:Breadcrumb
     * @param {jQuery} elem DOM Root element of
     */
    function BreadcrumbManager(elem) {
        this.breadcrumb = elem;
        this.hideHistory = [];
        this.hideHistory.elems = [];
        this.hideHistory.widths = [];
    }

    /**
     * @method
     * @alias getElements
     * @memberOf module:Breadcrumb.BreadcrumbManager
     * @param {$OrderedList} list Ordered List of 
     * breadcrumb elements wrapped by jquery
     */
    BreadcrumbManager.prototype.getElements = function($list) {
        var elements = [];

        $list.find("li").each(function() {
            elements.push(this);
        });

        return elements;
    };
    /**
     * Calculate width of component elements
     * @name calculateListElementsWidth
     * @function
     * @memberOf module:Breadcrumb.BreadcrumbManager
     * @return {number} widthSum
     */
    BreadcrumbManager.prototype.calculateListElementsWidth = function($list) {
        var widthSum = 0;

        $list.find(">li").each(function() {
            widthSum += $(this).width();
        });

        return widthSum;
    };
    /**
     * In case if breadcrumb elements do no fit the component width
     * several breadcrum items will be replaced by "..."
     * @name calculateWidth
     * @function
     * @memberOf module:Breadcrumb.BreadcrumbManager
     */
    BreadcrumbManager.prototype.calculateWidth = function() {
        var inst = this,
            $list = $(inst.breadcrumb).find("nav>ol"),
            listWidth = $list.width(),
            widthSum = this.calculateListElementsWidth($list),
            elements = this.getElements($list),
            $elementToHide,
            removeIndx = 0;

        var width = inst.hideHistory.widths[inst.hideHistory.widths.length - 1];
        if (listWidth > widthSum + width) {
            var elem = inst.hideHistory.elems.pop();
            inst.hideHistory.widths.pop();
            $(elem).removeClass("item-hide");
        }

        while (listWidth < widthSum && elements.length > 2) {
            removeIndx = Math.round(elements.length / 2) - 1;
            $elementToHide = $(elements[removeIndx]);

            inst.hideHistory.elems.push(elements[removeIndx]);
            inst.hideHistory.widths.push($elementToHide.width());
            $elementToHide.addClass("item-hide");

            widthSum = inst.calculateListElementsWidth($list);
            elements.splice(removeIndx, 1);
        }
    };
    /**
     * Calls in a case when added "breadcrumb-hide" class.
     * It`s do initial
     * [calculation]{@link module:Breadcrumb.BreadcrumbManager.calculateWidth}
     * of component width and also add 
     * [recalculation]{@link module:Breadcrumb.BreadcrumbManager.calculateWidth}
     * to window resize event.
     * @method
     * @alias module:Breadcrumb.BreadcrumbManager.init
     * @memberOf module:Breadcrumb.BreadcrumbManager
     */
    BreadcrumbManager.prototype.init = function() {
        var inst = this;

        inst.calculateWidth();
        $(window).resize(function() {
            inst.calculateWidth();
        });
    };
    /**
     * Default behaviour of breadcrumb.
     * In case if children exist it`s only add
     * "breadcrumb-navigation" class
     * @method
     * @alias makeNavigation
     * @memberOf module:Breadcrumb.BreadcrumbManager
     */
    BreadcrumbManager.prototype.makeNavigation = function() {
        var breadcrumb = $(this.breadcrumb),
            children = breadcrumb.find("li > ol");

        if (children.length > 0) {
            breadcrumb.addClass("breadcrumb-navigation");
        }
    };
    /**
     * initIstance methos create new instance of
     * [BreadcrumbManager]{@link module:Breadcrumb.BreadcrumbManager}
     * @memberOf module:Breadcrumb
     * @method
     * @param {jQuery} component Root DOM element of
     * breadcrumb component wrapped by jQuery
     * @alias module:Breadcrumb.initInstance
     */
    api.initInstance = function(component) {
        var breadcrumb = new BreadcrumbManager(component),
            $component = $(component);
        /** breadcrumb-hide - can be added manually in component setups */
        if ($component.hasClass("breadcrumb-hide")) {
            breadcrumb.init();
        } else {
            breadcrumb.makeNavigation();
        }
    };
    /**
     * init method calls in a loop for each
     * breadcrumb component on a page and run Breadcrumb's
     * [".initInstance"]{@link module:Breadcrumb.api.initInstance}  method.
     * @memberOf module:Breadcrumb
     * @alias module:Breadcrumb.init
     */
    api.init = function() {
        var breadcrumb = $(".breadcrumb:not(.initialized)");
        breadcrumb.each(function() {
            api.initInstance($(this));
            $(this).addClass("initialized");
        });
    };

    return api;
})(jQuery, document);

XA.register("breadcrumb", XA.component.breadcrumb);
