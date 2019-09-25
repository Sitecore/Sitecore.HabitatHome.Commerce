/**
 * Component Flip
 * @module Flip
 * @param  {jQuery} $ Instance of jQuery
 * @return {Object} List of flip methods
 */
XA.component.flip = (function($) {
    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Flip
     * @
     * */
    var api = {
            /**
             * equalSideHeight set css "min-height" property with value that
             * equal to size of bigger flip side 
             * @param {jQuery<DOMElement>} $el  Root DOM element of Flip component wrapped by jQuery 
             * @memberof module:Flip
             * @method equalSideHeight
             */
            equalSideHeight: function($el) {
                var side0 = $el.find(".Side0"),
                    side1 = $el.find(".Side1"),
                    slide0Height = this.calcSlideSizeInToggle(side0),
                    slide1Height = this.calcSlideSizeInToggle(side1),
                    maxHeight = Math.max(slide0Height, slide1Height);
                $el.find(".flipsides").css({ "min-height": maxHeight + "px" });
                side0.add(side1).css({ bottom: 0 });
            },
            /**
             * calcSlideSizeInToggle calculate size of slide content
             * @param {jQuery<DOMElement>} $slide Slide DOM Element of flip component
             * @memberof module:Flip
             * @method calcSlideSizeInToggle
             * @return {number} size
             */
            calcSlideSizeInToggle: function($slide) {
                var child = $slide.find(">div"),
                    size = 0;
                child.each(function(pos, el) {
                    size += $(el).outerHeight(true);
                });
                size += parseInt($slide.css("padding-top"));
                size += parseInt($slide.css("padding-bottom"));
                return size;
            },
            /**
             * equalSideHeightInToggle method that called from component toggle 
             * to make all slides inside same height
             * @param {jQuery<DOMElement>} $el  Root DOM element of Flip component wrapped by jQuery 
             * @memberof module:Flip
             * @method equalSideHeightInToggle
             */
            equalSideHeightInToggle: function($el) {
                var side0 = $el.find(".Side0"),
                    side1 = $el.find(".Side1"),
                    slide0Height = this.calcSlideSizeInToggle(side0),
                    slide1Height = this.calcSlideSizeInToggle(side1),
                    maxHeight = Math.max(slide0Height, slide1Height);
                $el.find(".flipsides").css({ "min-height": maxHeight + "px" });
                side0.add(side1).css({ bottom: 0 });
            }
        };

    function detectMobile() {
        return "ontouchstart" in window;
    }
    /**
     * calcHeightOnResize method call
     * ["equalSideHeight"]{@link module:Flip.equalSideHeight} method
     * for all initialized Flip components
     * @memberOf module:Flip
     * @method
     * @alias module:Flip.initInstance
     * @private
     */
    function calcHeightOnResize() {
        var flip = $(".flip.initialized");
        flip.each(function() {
            api.equalSideHeight($(this));
        });
    }
    /**
     * initInstance method bind toggling "active" class for component
     * an Flip element
     * @memberOf module:Flip
     * @method
     * @param {jQuery} component Root DOM element of flip component wrapped by jQuery
     * @alias module:Flip.initInstance
     */
    api.initInstance = function(component) {
        // Set tabindex=0 for first header
        component.find('[class*="Side0"]').attr("tabindex", "0");
        //
        if (component.hasClass("flip-hover") && !detectMobile()) {
            component.hover(
                function() {
                    component.addClass("active");
                },
                function() {
                    component.removeClass("active");
                }
            );
        } else {
            component.on("click", function() {
                component.toggleClass("active");
            });
        }
    };
    /**
     * init method calls in a loop for each
     * flip component on a page and run Flip's
     * ["initInstance"]{@link module:Flip.api.initInstance},
     * ["equalSideHeight"]{@link module:Flip.equalSideHeight} methods.
     * Added watcher to "resize" event on window that call
     * ["calcHeightOnResize"]{@link module:Flip.calcHeightOnResize}
     *
     * @memberOf module:Flip
     * @alias module:Flip.init
     */
    api.init = function() {
        var flip = $(".flip:not(.initialized)");
        $(window).on("resize", function() {
            calcHeightOnResize();
        });
        flip.each(function() {
            var $flipModule = $(this).find(".flipsides");
            $flipModule.find(".Side0").attr("tabindex", "0");
            api.initInstance($(this));
            $(this).addClass("initialized");
            api.equalSideHeight($(this));
        });
    };
    return api;
})(jQuery, document);

XA.register("flip", XA.component.flip);
