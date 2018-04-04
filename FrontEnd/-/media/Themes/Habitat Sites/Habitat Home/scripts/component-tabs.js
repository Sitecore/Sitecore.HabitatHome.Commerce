XA.component.tabs = (function($) {

    var pub = {},
        instance;

    function pageEditor() {
        if ($('body').hasClass('on-page-editor')) {
            Sitecore.PageModes.ChromeManager.resetChromes(); //page editor lines fix
        }
    }
    var isFlipInside = function(instance) {
        return !!instance.find('.component.flip').length
    }

    function tabsScrollable($tabsScroll) {
        var speed = 150; //tabs scroll speed

        function initNavScroll($tabsNav) {
            var sum = 0,
                maxHeight = 0;

            if ($tabsNav.length) {
                $tabsNav.parent().find(".prev").remove();
                $tabsNav.parent().find(".next").remove();
                $tabsNav.unwrap();

                $tabsNav.css("width", "auto");
                $tabsNav.css("height", "auto");
                $tabsNav.css("left", 0);
            }


            $tabsNav.find("li").each(function() {
                sum += $(this).outerWidth(true);
            });


            $tabsNav.find("li").each(function() {
                maxHeight = Math.max(maxHeight, $(this).height());
            });


            $tabsNav.wrap("<div class='wrapper'>");
            $("<div class='next tab-slider'>></div>").insertAfter($tabsNav);
            $("<div class='prev tab-slider'><</div>").insertBefore($tabsNav);


            $tabsNav.parent().css("height", parseInt(maxHeight, 10));
            $tabsNav.parent().find(".tab-slider").css("height", parseInt(maxHeight, 10) - 2);
            //fix for 8111
            sum += 10;
            if (sum > $tabsNav.parent().width()) {
                $tabsNav.parent().find(".prev").hide();
                $tabsNav.width(sum);
            } else {
                $tabsNav.parent().find(".prev").hide();
                $tabsNav.parent().find(".next").hide();
            }
        }


        function bindPrevNextEvents(current, $tabsNav) {
            current.find(".prev").click(function() {
                var left = parseInt($tabsNav.css("left"), 10);
                left += speed;

                if (left > 0) {
                    left = 0;
                    $tabsNav.stop().animate({
                        "left": left
                    });

                    $tabsNav.parent().find(".prev").hide();
                    $tabsNav.parent().find(".next").show();
                } else {
                    $tabsNav.stop().animate({
                        "left": left
                    });

                    $tabsNav.parent().find(".prev").show();
                    $tabsNav.parent().find(".next").show();
                }
            });


            current.find(".next").click(function() {
                var left = parseInt($tabsNav.css("left"), 10),
                    navWidth = $tabsNav.width(),
                    navParentWidth = $tabsNav.parent().width();

                left -= speed;

                if ((navWidth + left) < navParentWidth) {
                    left = navWidth - navParentWidth + 20;
                    $tabsNav.stop().animate({
                        "left": -left
                    });

                    $tabsNav.parent().find(".prev").show();
                    $tabsNav.parent().find(".next").hide();
                } else {
                    $tabsNav.stop().animate({
                        "left": left
                    });

                    $tabsNav.parent().find(".prev").show();
                    $tabsNav.parent().find(".next").show();
                }
            });
        }

        function bindChangeTabs($tabsNav, $tabsContainer) {
            $tabsNav.find("li").click(function() {
                var index = $(this).index();

                $(this).addClass("active");
                $(this).siblings().removeClass("active");

                $tabsContainer.find(".tab").removeClass("active");
                $tabsContainer.find(".tab:eq(" + index + ")").addClass("active");
                if (isFlipInside(instance)) {
                    try {
                        XA.component.flip.equalSideHeight(instance)
                    } catch (e) {
                        /* eslint-disable no-console */
                        console.warn('Error during calculation height of Flip list in toggle'); // jshint ignore:line
                        /* eslint-enable no-console */

                    }
                }

                pageEditor();

                return false;
            });
        }

        function initTabsScrollable($tabs) {
            $tabs.each(function() {
                var $tabsNav = $(this).find(".tabs-heading"),
                    $tabsContainer = $(this).find(".tabs-container");

                $tabsNav.find("li:first-child").addClass("active");
                $tabsContainer.find(".tab:eq(0)").addClass("active");

                bindChangeTabs($tabsNav, $tabsContainer);
                initNavScroll($tabsNav);
                bindPrevNextEvents($(this), $tabsNav);
            });
        }

        initTabsScrollable($tabsScroll);
        $(window).resize(function() {
            initTabsScrollable($tabsScroll);
        });
    }

    function disableEmptyTabs($tabs) {
        var emptyPlaceholders = $tabs.find(".scEmptyPlaceholder"),
            i, length, h,
            parent,
            $placeholder,
            placeholderId,
            placeholderKey,
            editFrame,
            chromeKey;

        for (i = 0, length = emptyPlaceholders.length; i < length; i++) {
            $placeholder = $(emptyPlaceholders[i]);
            placeholderId = $placeholder.attr("sc-placeholder-id");

            //disable edit frames
            var placeholderCodeTag = $placeholder.siblings("code[chrometype='placeholder'][id='" + placeholderId + "']");
            try {
                placeholderKey = placeholderCodeTag.attr("key");
                if (typeof(placeholderKey) === "string") {
                    editFrame = Sitecore.PageModes.ChromeManager.chromes().filter(function(chrome) {
                        chromeKey = chrome.openingMarker();
                        if (!chromeKey) {
                            return false;
                        }

                        chromeKey = chromeKey.attr("key");
                        return typeof(chromeKey) !== "undefined" && chromeKey.indexOf(placeholderKey) === 0;
                    });

                    editFrame[0].type.chrome.data.custom.editable = "false";
                } else {
                    throw 666;
                }
            } catch (e) { //fallback - delete code tags to prevent inserting into placeholders
                $placeholder.siblings("code[chrometype='placeholder']").remove();
                /* eslint-disable no-console */
                console.log("Could not disable editing for placeholder", e);
                /* eslint-enable no-console */
            }

            //adjust height, and remove placeholder "dashed" background
            parent = $placeholder.parents(".tabs-heading");
            if (parent.length !== 0) {
                h = parseInt(parent.css("font-size")) * 2 || ($placeholder.height() / 3);
                $placeholder.append($("<p>[No components in header]</p>"));
            } else {
                h = $placeholder.height();
                $placeholder.append($("<p>[No components in tab]</p>"));
            }
            h += "px";
            $placeholder.removeClass("scEmptyPlaceholder");
            $placeholder.css("height", h);

            //set edit here text

        }
    }



    pub.init = function() {
        var $tabs = $(".tabs:not(.initialized)");
        $tabs.each(function() {
            var $tabModule = $(this).find(".tabs-inner");
            instance = $(this);
            if ($(this).hasClass("tabs-scrollable")) {
                tabsScrollable($(this));
            } else {
                $tabModule.each(function() {
                    var $tabNav = $(this).find(".tabs-heading > li"),
                        $tabs = $(this).find("> .tabs-container > .tab");
                    $tabNav.first().addClass("active");
                    $tabs.first().addClass("active");
                    $tabNav.click(function(event) {
                        var index = $(this).index();
                        $(this).siblings().removeClass("active");
                        $(this).parent().parent().find("> .tabs-container > .tab").removeClass("active");
                        $(this).addClass("active");
                        $($(this).parent().parent().children(".tabs-container").children(".tab").eq(index)).addClass("active");
                        if (isFlipInside(instance)) {
                            try {
                                XA.component.flip.equalSideHeight(instance)
                            } catch (e) {
                                /* eslint-disable no-console */
                                console.warn('Error during calculation height of Flip list in toggle'); // jshint ignore:line
                                /* eslint-enable no-console */

                            }
                        }
                        pageEditor();
                        event.preventDefault();
                    });
                });
            }

            disableEmptyTabs($tabModule);
            if (isFlipInside(instance)) {
                try {
                    XA.component.flip.equalSideHeight(instance)
                } catch (e) {
                    /* eslint-disable no-console */
                    console.warn('Error during calculation height of Flip list in toggle'); // jshint ignore:line
                    /* eslint-enable no-console */

                }
            }
            $(this).addClass("initialized");
        });
    }

    return pub;
}(jQuery));

XA.register("tabs", XA.component.tabs);