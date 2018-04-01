XA.component.navigation = (function($) {

    var timeout = 200,
        timer = 0,
        submenu,
        dropDownEvents = {
            show: function(sm) {
                this.debounce();
                if (submenu) {
                    submenu.closest('li').removeClass("show");
                }
                submenu = sm;
                submenu.closest('li').addClass("show");
            },
            debounce: function() {
                if (timer) {
                    clearTimeout(timer);
                    timer = null;
                }
            },
            hide: function() {
                if (submenu) {
                    submenu.closest('li').removeClass("show")
                }
            },
            queueHide: function() {
                timer = setTimeout(function() {
                    dropDownEvents.hide();
                }, timeout);
            },
            focus: function() {
                $(this).closest('li').siblings().removeClass("show");
                $(this).closest('li').addClass("show");
            },
            blur: function() {
                if ($(this).closest('li').is(".last")) {
                    $(this).parents(".rel-level1").removeClass("show");
                }
            }

        };

    function dropDownNavigation(navi) {
        navi.on("mouseover", ".rel-level1 > a, .rel-level1 >.navigation-title>a", function() {
            $(this).closest('li').removeClass("show");
            $(this).closest('li').siblings().removeClass("show");
            var elem = $(this).closest('li').find(">ul");
            dropDownEvents.show(elem);
        });
        navi.on("mouseleave", ".rel-level1 > a, .rel-level1 >.navigation-title>a",
            dropDownEvents.queueHide);
        navi.on("mouseover", ".rel-level1 > ul", dropDownEvents.debounce);
        navi.on("mouseleave", ".rel-level1 > ul", dropDownEvents.queueHide);
        navi.on("focus", ".rel-level1 > a, .rel-level1 >.navigation-title>a",
            dropDownEvents.focus);
        navi.on("blur", ".rel-level2 > a", dropDownEvents.blur);

        navi.find(".rel-level1").each(function() {
            if ($(this).find("ul").length) {
                $(this).addClass("submenu");
            }
        });

        navi.find(".rel-level2").each(function() {
            //if level2 menu have children
            if ($(this).parents('#header') > 0) {
                if ($(this).find("ul").length) {
                    $(this).addClass("submenu");
                    $(this).parents('.rel-level1').addClass('wide-nav');
                }
            }


            //if level2 menu should be navigation-image variant
            if ($(this).find('> img').length) {
                $(this).addClass("submenu navigation-image");
            }
        });
    }

    function mobileNavigation(navi) {

        function checkChildren(nav) {
            nav.find(".rel-level1").each(function() {
                if (!$(this).find("ul").length) {
                    $(this).addClass("no-child");
                }
            });
        }

        function bindEvents(nav) {
            nav.find(".rel-level1").on("click", function(e) {
                var navlvl = $(this),
                    menuParent = navlvl.parents('.navigation');

                if (menuParent.hasClass('navigation-mobile')) {
                    if (!$(e.target).is("a")) {
                        if (navlvl.hasClass("active")) {
                            navlvl.find("ul").slideToggle(function() {
                                navlvl.removeClass("active");
                            });
                        } else {
                            navlvl.find("ul").slideToggle(function() {
                                navlvl.addClass("active");
                            });
                        }
                    }
                }
            });

            nav.find(".rel-level1 > a").on("focus", function() {
                $(this).siblings("ul").slideDown();
                $(this).closest('li').siblings().find("ul").slideUp();
            });
        }

        checkChildren(navi);
        bindEvents(navi);
    }

    function toggleIcon(toggle) {
        $(toggle).on("click", function() {
            $(this).parents(".navigation").toggleClass("active");
            $(this).toggleClass("active");
        });
    }

    var api = {};

    api.init = function() {
        var navigation = $(".navigation:not(.initialized)");

        navigation.each(function() {
            if ($(this).hasClass("navigation-main")) {
                dropDownNavigation($(this));
                mobileNavigation($(this));
            } else if ($(this).hasClass("navigation-mobile")) {
                mobileNavigation($(this));
            }

            $(this).addClass("initialized");
        });

        var toggle = $(".mobile-nav:not(.initialized)");
        toggle.each(function() {
            toggleIcon(this);
            $(this).addClass("initialized");
        });

    };

    return api;
}(jQuery, document));

XA.register("navigation", XA.component.navigation);