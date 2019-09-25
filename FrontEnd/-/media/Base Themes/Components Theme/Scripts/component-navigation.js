/**
 * Component Navigation
 * @module Navigation
 * @param  {jQuery} $ Instance of jQuery
 * @return {Object} List of navigation methods
 */
XA.component.navigation = (function($) {
    var timeout = 200,
        timer = 0,
        submenu,
        dropDownEvents = {
            show: function(sm) {
                this.debounce();
                submenu = sm;
                submenu.closest("li").addClass("show");
            },
            debounce: function() {
                if (timer) {
                    clearTimeout(timer);
                    timer = null;
                }
            },
            hide: function() {
                if (submenu) {
                    submenu.closest("li").removeClass("show");
                }
            },
            queueHide: function() {
                timer = setTimeout(function() {
                    dropDownEvents.hide();
                }, timeout);
            },
            focus: function() {
                $(this)
                    .closest("li")
                    .siblings()
                    .removeClass("show");
                $(this)
                    .closest("li")
                    .addClass("show");
            },
            blur: function() {
                if (
                    $(this)
                        .closest("li")
                        .is(".last")
                ) {
                    $(this)
                        .parents(".rel-level1")
                        .removeClass("show");
                }
            }
        };

    /**
     * Prepare DOM structure of Navigation component and<br/>
     * add Event listeners of showing and hiding
     * different levels of navigation.
     * @method
     * @alias module:Navigation.dropDownNavigation
     * @memberOf module:Navigation
     * breadcrum elements wrapped by jquery
     * @param {jQuery} navi Root DOM element of
     * navigation component wrapped by jQuery
     * @private
     */
    function dropDownNavigation(navi) {
        navi.on(
            "mouseover",
            ".rel-level1 > a, .rel-level1 >.navigation-title>a, .rel-level2 >.navigation-title>a",
            function() {
                $(this)
                    .closest("li")
                    .siblings()
                    .removeClass("show");
                $(this)
                    .closest("li.rel-level1")
                    .siblings()
                    .removeClass("show");
                $(this)
                    .closest("li.rel-level1")
                    .siblings()
                    .find(".show")
                    .removeClass("show");
                var elem = $(this)
                    .closest("li")
                    .find(">ul");
                dropDownEvents.show(elem);
            }
        );
        navi.on(
            "mouseleave",
            ".rel-level1 > a, .rel-level1 >.navigation-title",
            dropDownEvents.queueHide
        );
        navi.on("mouseover", ".rel-level1 > ul", dropDownEvents.debounce);
        navi.on("mouseleave", ".rel-level1 > ul", dropDownEvents.queueHide);
        navi.on(
            "focus",
            ".rel-level1 > a, .rel-level1 >.navigation-title, .rel-level1 >.navigation-title",
            dropDownEvents.focus
        );
        navi.on("blur", ".rel-level2 > a", dropDownEvents.blur);
        navi.on("mouseleave", function() {
            $(this)
                .find(".show")
                .removeClass("show");
        });
        navi.find(".rel-level1").each(function() {
            if ($(this).find("ul").length) {
                $(this).addClass("submenu");
            }
        });

        navi.find(".rel-level2").each(function() {
            //if level2 menu have children
            if ($(this).parents("#header") > 0) {
                if ($(this).find("ul").length) {
                    $(this).addClass("submenu");
                    $(this)
                        .parents(".rel-level1")
                        .addClass("wide-nav");
                }
            }

            //if level2 menu should be navigation-image variant
            if ($(this).find("> img").length) {
                $(this).addClass("submenu navigation-image");
            }
        });

        navi.on("click", ".sxaToogleNavBtn", function() {
            var $this = jQuery(this);
            $this.find(".sxaWrappedList").toggleClass("hidden");
        });
    }
    /**
     * Prepare DOM structure of Mobile Navigation component and<br/>
     * add Event listeners of showing and hiding
     * different levels of navigation.
     * @method
     * @alias module:Navigation.mobileNavigation
     * @memberOf module:Navigation
     * breadcrum elements wrapped by jquery
     * @param {jQuery} navi Root DOM element of
     * navigation component wrapped by jQuery
     * @private
     */
    function mobileNavigation(navi) {
        function checkChildren(nav) {
            nav.find(".rel-level1").each(function() {
                if (!$(this).find("ul").length) {
                    $(this).addClass("no-child");
                }
            });
        }

        function bindEvents(nav) {
            nav.find(".navigation-title").on("click", function(e) {
                var navlvl = $(this).closest("li"),
                    menuParent = navlvl.parents(".navigation");

                if (menuParent.hasClass("navigation-mobile")) {
                    if (!$(e.target).is("a")) {
                        if (navlvl.hasClass("active")) {
                            navlvl.find(">ul").slideToggle(function() {
                                navlvl.removeClass("active");
                            });
                        } else {
                            navlvl.find(">ul").slideToggle(function() {
                                navlvl.addClass("active");
                            });
                        }
                    }
                }
            });

            nav.find(".rel-level1").on("focus", function() {
                $(this)
                    .siblings("ul")
                    .slideDown();
                $(this)
                    .closest("li")
                    .siblings()
                    .find("ul")
                    .slideUp();
            });
        }
        checkChildren(navi);
        bindEvents(navi);
    }

    /**
     * @method
     * @alias module:Navigation.toggleIcon
     * @memberOf module:Navigation
     * @param {$} toggle
     * navigation elements wrapped by jquery
     * @private
     */
    function toggleIcon(toggle) {
        $(toggle).on("click", function() {
            $(this)
                .parents(".navigation")
                .toggleClass("active");
            $(this).toggleClass("active");
        });
    }

    /**
     * In this object stored all public api methods
     * @type {Object.<Methods>}
     * @memberOf module:Navigation
     * */
    var api = {};

    /**
     * initIstance method. <br/>
     * If added <b>navigation-main</b> class
     * called [dropDownNavigation]{@link module:Navigation.dropDownNavigation},
     * [mobileNavigation]{@link module:Navigation.mobileNavigation} methods. <br/>
     * If added <b>navigation-mobile</b> class
     * called only [mobileNavigation]{@link module:Navigation.mobileNavigation},
     * @memberOf module:Navigation
     * @method
     * @param {jQuery} component Root DOM element of
     * navigation component wrapped by jQuery
     * @alias module:Navigation.initInstance
     */
    api.initInstance = function(component) {
        if (component.hasClass("navigation-main")) {
            dropDownNavigation(component);
            mobileNavigation(component);
        } else if (component.hasClass("navigation-mobile")) {
            mobileNavigation(component);
        }
    };
    /**
     * init method calls [initInstance]{@link module:Navigation.initInstance} method
     * in a loop for each navigation component on a page. <br/>
     *
     * For navigation with setted <b>mobile-nav</b> class
     * calls [toggleIcon]{@link module:Navigation.toggleIcon} method
     *
     * @memberOf module:Navigation
     * @alias module:Navigation.init
     */
    api.init = function() {
        var navigation = $(".navigation:not(.initialized)");
        navigation.each(function() {
            api.initInstance($(this));
            $(this).addClass("initialized");
        });

        var toggle = $(".mobile-nav:not(.initialized)");
        toggle.each(function() {
            toggleIcon(this);
            $(this).addClass("initialized");
        });
    };

    return api;
})(jQuery, document);

XA.register("navigation", XA.component.navigation);
