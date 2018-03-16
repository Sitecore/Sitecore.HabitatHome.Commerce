/* global _:true */
XA.component.accordions = (function($) {
    var pub = {};

    function pageEditor() {
        if ($('body').hasClass('on-page-editor')) {
            Sitecore.PageModes.ChromeManager.resetChromes(); //page editor lines fix
        }
    }

    var toogleEvents = {
        focus: function() {
            $(this).addClass("show");
        },
        blur: function() {
            $(this).removeClass("show");
        }
    };

    function headerBackground(header) {
        var backgroundIcon = $(header),
            background = $(header).find('img'),
            backgroundSrc = background.attr('src');

        if (backgroundSrc) {
            backgroundIcon.parents('.accordion').addClass('accordion-image');

            backgroundIcon.css({
                'background': 'url(' + backgroundSrc + ')',
                'background-repeat': 'no-repeat',
                'background-size': 'cover',
                'background-position': '50% 100%'
            });

            background.hide();
        }
    }

    function calcSlideSize(acc) {
        var accordionWidth = $(acc).width(),
            accordionItems = $(acc).find('.item'),
            maxHeight = 0;

        _.each(accordionItems, function(item) {
            var itemContent = $(item).find('.toggle-content'),
                itemHeader = $(item).find('.toggle-header'),
                slideWidth = accordionWidth - (accordionItems.length * itemHeader.outerWidth());

            if ($(item).hasClass('active')) {
                $(item).css({
                    'width': slideWidth
                });
            }

            //width
            itemContent.css({
                'width': ($(acc).hasClass('accordion-image')) ? slideWidth + itemHeader.outerWidth() : slideWidth - itemHeader.outerWidth() - parseInt(itemHeader.css('padding'))
            });

            //height
            if ($(item).find('.toggle-content').height() > maxHeight) {
                maxHeight = $(item).find('.toggle-content').height();
            }
        });
    }

    function animateHorizontal(properties) {
        var accordion = $(this).parents('.accordion'),
            panel = $(this).closest('.item'),
            header = panel.find('.toggle-header'),
            content = panel.find('.toggle-content'),
            items = accordion.find('.item'),
            siblings = panel.siblings(),
            siblingsContent = siblings.find('.toggle-content');

        siblings.stop(true).animate({
            "width": 0
        }, properties.speed, properties.easing, function() {
            siblingsContent.css({
                "display": "none"
            });
        });

        if (panel.hasClass('active')) {

            var slideWidth = accordion.hasClass('accordion-image') ? content.outerWidth() : (accordion.outerWidth() - ((items.length - 1) * panel.outerWidth() + 2)),
                contentWidth = accordion.hasClass('accordion-image') ? slideWidth : slideWidth - header.outerWidth();

            panel.stop(true).animate({
                "width": slideWidth
            }, properties.speed, properties.easing, function() {

            });


            content.css({
                "width": contentWidth,
                "display": "block"
            });

        } else {

            panel.stop(true).animate({
                "width": 0
            }, properties.speed, properties.easing, function() {
                content.css({
                    "display": "none"
                });
            });
        }
    }

    function accordion(acc, properties) {
        var ev = 'click',
            $body = $('body'),
            toggleHeader = acc.find('.toggle-header');

        if (properties.expandOnHover) {
            if (!$body.hasClass('on-page-editor')) {
                ev = 'mouseenter';
            } else {
                ev = 'click';
            }

        }

        toggleHeader.on("mouseover", toogleEvents.focus);
        toggleHeader.on("mouseleave", toogleEvents.blur);
        toggleHeader.on("focus", toogleEvents.focus);
        toggleHeader.on("blur", toogleEvents.blur);
        toggleHeader.on("keyup", function(e) {
            if (e.keyCode == 13) {
                $(this).click();
            }
        });


        if (acc.hasClass('accordion-horizontal')) {
            //calc slide width
            $(window).on('load', function() {
                calcSlideSize(acc);
            });

            _.each(toggleHeader, function(header) {
                headerBackground(header);
            });

        }

        if ($('body').hasClass('on-page-editor')) {
            toggleHeader = toggleHeader.find('.component-content');
        }
        toggleHeader.on(ev, function() {
            var accordion = $(this).parents('.accordion'),
                panel = $(this).closest('.item'),
                $body = $('body'),
                content = panel.find('.toggle-content'),
                siblings = panel.siblings(),
                siblingsContent = siblings.find('.toggle-content'),
                timeoutId;

            var callback = function() {
                pageEditor();
            };

            if (!properties.expandOnHover) {
                if (properties.canOpenMultiple) {
                    panel.toggleClass('active');
                    if (accordion.hasClass("accordion-horizontal")) {
                        animateHorizontal.call(this, properties);
                    } else {

                    content.stop().slideToggle({
                            duration: properties.speed,
                            easing: properties.easing
                        },
                        callback);
                    }


                } else {
                    if (properties.canToggle) {


                        siblings.removeClass('active');

                        panel.toggleClass('active');

                        if (accordion.hasClass("accordion-horizontal")) {
                            //animation for horizontal accordion
                            animateHorizontal.call(this, properties);

                        } else {
                            siblingsContent.stop().slideUp({
                                    duration: properties.speed,
                                    easing: properties.easing
                                },
                                callback);

                            content.stop().slideToggle({
                                    duration: properties.speed,
                                    easing: properties.easing
                                },
                                callback);
                        }


                    } else {
                        siblings.removeClass('active');
                        siblingsContent.slideUp({
                                duration: properties.speed,
                                easing: properties.easing
                            },
                            callback);

                        panel.addClass('active');
                        content.slideDown({
                                duration: properties.speed,
                                easing: properties.easing
                            },
                            callback);
                    }

                }
            } else {
                if (properties.canToggle) {
                    panel.unbind('mouseleave');

                    if (!$body.hasClass('on-page-editor')) {
                        panel.on('mouseleave', function() {
                            timeoutId = setTimeout(function() {
                                panel.removeClass('active');
                                if (accordion.hasClass("accordion-horizontal")) {
                                    //animation for horizontal accordion
                                    animateHorizontal.call(this, properties);

                                } else {
                                    content.stop().slideUp({
                                            duration: properties.speed,
                                            easing: properties.easing
                                        },
                                        callback);

                                }
                            }, 300);

                        });
                    } else {
                        $body.on('click', function() {
                            timeoutId = setTimeout(function() {
                                panel.removeClass('active');
                                if (accordion.hasClass("accordion-horizontal")) {
                                    //animation for horizontal accordion
                                    animateHorizontal.call(this, properties);

                                } else {
                                    content.stop().slideUp({
                                            duration: properties.speed,
                                            easing: properties.easing
                                        },
                                        callback);

                                }
                            }, 300);

                        });
                    }



                    content.unbind('mouseenter');
                    content.on('mouseenter', function() {
                        clearTimeout(timeoutId);
                    });
                }
                if (accordion.hasClass("accordion-horizontal")) {
                    siblings.removeClass('active');

                    panel.toggleClass('active');
                    //animation for horizontal accordion
                    animateHorizontal.call(this, properties);

                } else {
                    siblings.removeClass('active');
                    siblingsContent.stop().slideUp({
                            duration: properties.speed,
                            easing: properties.easing
                        },
                        callback);

                    panel.addClass('active');
                    content.slideDown({
                            duration: properties.speed,
                            easing: properties.easing
                        },
                        callback);
                }
            }
        });
    }

    function disableEmptyAccordions($accordion) {
        var emptyPlaceholders = $accordion.find(".item .scEmptyPlaceholder"),
            i, length,
            placeholderCodeTag,
            parent,
            $placeholder,
            placeholderId,
            placeholderKey,
            editFrame,
            chromeKey,
            h;

        for (i = 0, length = emptyPlaceholders.length; i < length; i++) {
            $placeholder = $(emptyPlaceholders[i]);
            placeholderId = $placeholder.attr("sc-placeholder-id");

            //disable edit frames
            placeholderCodeTag = $placeholder.siblings("code[chrometype='placeholder'][id='" + placeholderId + "']");
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
                /* eslint-disable no-alert, no-console */
                console.log("Could not disable editing for placeholder", e);
                /* eslint-enable no-alert, no-console */
            }

            //adjust height, and remove placeholder "dashed" background
            parent = $placeholder.parents(".toggle-header");
            if (parent.length !== 0) {
                h = parseInt(parent.css("font-size")) * 2 || ($placeholder.height() / 3);
                $placeholder.append($("<p>[No components in header]</p>"));
            } else {
                h = $placeholder.height();
                $placeholder.append($("<p>[No components in section]</p>"));
            }
            h += "px";
            $placeholder.removeClass("scEmptyPlaceholder");
            $placeholder.css("height", h);
        }

    }

    pub.init = function() {
        var accordions = $('.accordion:not(.initialized)');

        accordions.each(function() {
            var properties = $(this).data('properties'),
                acc = $(this);

            if ($(this).hasClass('toggle')) {
                $.extend(properties, {
                    'canToggle': true,
                });
            }

            acc.find('.toggle-content').hide();
            if (properties.expandedByDefault) {
                acc.find('li:first-child').addClass('active');
                acc.find('li:first-child').find('.toggle-content').show();
            }

            accordion(acc, properties);
            disableEmptyAccordions(acc);
            acc.addClass('initialized');
        });
    };

    return pub;

}(jQuery));

XA.register('accordions', XA.component.accordions);