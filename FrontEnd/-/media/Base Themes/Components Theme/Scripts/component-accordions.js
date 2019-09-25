/* global _:true */
/**
 * Component Accordion
 * @module Accordion
 * @param  {jQuery} $ Instance of jQuery
 * @return {Object} List of accordion methods
 */
XA.component.accordions = (function($) {
    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Accordion
     * */
    var api = {};
    /**
     * toogleEvents list of toggle events
     * @memberOf module:Accordion
     * @private
     */
    var toogleEvents = {
        focus: function() {
            $(this).addClass("show");
        },
        blur: function() {
            $(this).removeClass("show");
        }
    };
    /**
     * headerBackground method set up background image for toggle-header
     * @memberOf module:Accordion
     * @param {DomElement} header toggle-header DOM element of accordion component
     * @alias module:Accordion.headerBackground
     * @private
     */
    function headerBackground(header) {
        var backgroundIcon = $(header),
            background = $(header).find("img"),
            backgroundSrc = background.attr("src");

        if (backgroundSrc) {
            backgroundIcon.parents(".accordion").addClass("accordion-image");

            backgroundIcon.css({
                background: "url(" + backgroundSrc + ")",
                "background-repeat": "no-repeat",
                "background-size": "cover",
                "background-position": "50% 100%"
            });

            background.hide();
        }
    }
    /**
     * calcSlideSize method calculate and set up width of accordion items
     * @memberOf module:Accordion
     * @param {DomElement} acc Root Dom element of accordion component
     * @alias module:Accordion.calcSlideSize
     * @private
     */
    function calcSlideSize(acc) {
        var accordionWidth = $(acc).width(),
            accordionItems = $(acc).find(".item"),
            maxHeight = 0;

        _.each(accordionItems, function(item) {
            var itemContent = $(item).find(".toggle-content"),
                itemHeader = $(item).find(".toggle-header"),
                slideWidth =
                    accordionWidth -
                    accordionItems.length * itemHeader.outerWidth();

            if ($(item).hasClass("active")) {
                $(item).css({
                    width: slideWidth
                });
            }

            //width
            itemContent.css({
                width: $(acc).hasClass("accordion-image")
                    ? slideWidth + itemHeader.outerWidth()
                    : slideWidth -
                      itemHeader.outerWidth() -
                      parseInt(itemHeader.css("padding"))
            });

            //height
            if (
                $(item)
                    .find(".toggle-content")
                    .height() > maxHeight
            ) {
                maxHeight = $(item)
                    .find(".toggle-content")
                    .height();
            }
        });
    }

    /**
     * animateHorizontal method set up animation for horizontal oriented
     * accordion component
     * @memberOf module:Accordion
     * @param {Object} properties list of properties for accordion component
     * @alias module:Accordion.animateHorizontal
     */
    api.animateHorizontal = function(properties) {
        var accordion = $(this).parents(".accordion"),
            panel = $(this).closest(".item"),
            header = panel.find(".toggle-header"),
            content = panel.find(".toggle-content"),
            items = accordion.find(".item"),
            siblings = panel.siblings(),
            siblingsContent = siblings.find(".toggle-content");

        panel.toggleClass("active");
        siblings.removeClass("active");
        siblings.stop(true).animate(
            {
                width: 0
            },
            properties.speed,
            properties.easing,
            function() {
                siblingsContent.css({
                    display: "none"
                });
            }
        );

        if (panel.hasClass("active")) {
            var slideWidth = accordion.hasClass("accordion-image")
                    ? content.outerWidth()
                    : accordion.width() -
                      ((items.length - 1) * panel.outerWidth() + 2),
                contentWidth = accordion.hasClass("accordion-image")
                    ? slideWidth
                    : slideWidth - header.outerWidth();

            panel.stop(true).animate(
                {
                    width: slideWidth
                },
                properties.speed,
                properties.easing,
                function() {}
            );

            content.css({
                width: contentWidth,
                display: "block"
            });
        } else {
            panel.stop(true).animate(
                {
                    width: 0
                },
                properties.speed,
                properties.easing,
                function() {
                    content.css({
                        display: "none"
                    });
                }
            );
        }
    };
    /**
     * getQueryParamKey get an Id of accordion component
     * @memberOf module:Accordion
     * @param {jQuery} accordions  Root Dom element of accordion component
     * @private
     * @return {null|string} return id of accordion component
     */
    function getQueryParamKey(accordions) {
        var firstAccordionId = accordions[0].id;
        if (accordions.length > 0 && firstAccordionId != "") {
            return firstAccordionId.toLocaleLowerCase();
        }
        return null;
    }
    /**
     * accordion method setting up watchers that animate accordion slide
     * @memberOf module:Accordion
     * @method
     * @private
     * @param {jQuery} acc Root DOM element of archive component wrapped by jQuery
     * @param {Object} prop list of properties for accordion component
     * @alias module:Accordion.accordion
     */
    function accordion(acc, properties) {
        var ev = "click",
            toggleHeader = acc.find(".toggle-header");

        if (properties.expandOnHover) {
            ev = "mouseenter";
        }

        toggleHeader.on("mouseover", toogleEvents.focus);
        toggleHeader.on("mouseleave", toogleEvents.blur);
        toggleHeader.on("focus", toogleEvents.focus);
        toggleHeader.on("blur", toogleEvents.blur);
        toggleHeader.on("keyup", function(e) {
            if (e.keyCode == 13 || e.keyCode == 32) {
                $(this).click();
            }
        });

        if (acc.hasClass("accordion-horizontal")) {
            //calc slide width
            $(document).ready(function() {
                calcSlideSize(acc);
            });

            _.each(toggleHeader, function(header) {
                headerBackground(header);
            });
        }

        toggleHeader.on(ev, function() {
            var $this = $(this),
                panel = $this.closest(".item"),
                accordion = $this.parents(".accordion"),
                content = panel.find(".toggle-content"),
                siblings = panel.siblings(),
                siblingsContent = siblings.find(".toggle-content");

            // Horizontal animation
            if (accordion.hasClass("accordion-horizontal")) {
                api.animateHorizontal.call(this, properties);
                // Vertical animation
            } else {
                // Close all other items if open
                // multiple option disabled
                if (!properties.canOpenMultiple) {
                    siblings.removeClass("active");
                    siblingsContent.stop().slideUp({
                        duration: properties.speed,
                        easing: properties.easing
                    });
                }

                panel.toggleClass("active");
                // Toggle state of selected item
                // If toggle state is enabled
                if (properties.canToggle) {
                    content.slideToggle({
                        duration: properties.speed,
                        easing: properties.easing
                    });
                } else {
                    content.slideDown({
                        duration: properties.speed,
                        easing: properties.easing
                    });
                }
            }
        });
    }
    /**
     * initInstance method prepare Dom elements according accordion setup
     * and call ["accordion"]{@link module:Accordion.accordion} method
     * @memberOf module:Acordion
     * @method
     * @param {jQuery} component Root DOM element of archive component wrapped by jQuery
     * @param {Object} prop list of properties for accordion component
     * @alias module:Accordion.initInstance
     */
    api.initInstance = function(component, prop) {
        // Set tabindex=0 for first header
        component
            .find(".toggle-header")
            .eq(0)
            .attr("tabindex", "0");
        //
        if (component.hasClass("toggle")) {
            $.extend(prop, {
                canToggle: true
            });
        }
        component.find(".toggle-content").hide();
        var current = XA.queryString.getQueryParam(getQueryParamKey(component));
        if (current != null) {
            var arr = current.split(",");
            var items = component.find("li");
            for (var index = 0; index < items.length; index++) {
                var element = items[index];
                if (arr.indexOf(index + "") > -1) {
                    $(element).addClass("active");
                    $(element)
                        .find(".toggle-content")
                        .show();
                }
            }
        } else if (prop.expandedByDefault) {
            component.find("li:first-child").addClass("active");
            component
                .find("li:first-child")
                .find(".toggle-content")
                .show();
        }
        accordion(component, prop);
    };
    /**
     * init method calls in a loop for each
     * accordions component on a page and run Accordions's
     * [".initInstance"]{@link module:Accordion.initInstance} method.
     * @memberOf module:Accordion
     * @alias module:Accordion.init
     */
    api.init = function() {
        var accordions = $(".accordion:not(.initialized)");
        accordions.each(function() {
            var properties = $(this).data("properties"),
                acc = $(this);
            api.initInstance(acc, properties);
            acc.addClass("initialized");
        });
    };

    return api;
})(jQuery);

XA.register("accordions", XA.component.accordions);
