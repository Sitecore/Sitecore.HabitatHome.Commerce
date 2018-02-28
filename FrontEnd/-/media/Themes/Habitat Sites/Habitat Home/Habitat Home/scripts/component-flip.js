XA.component.flip = (function($) {
    var api = {
            getSideSortByHeight: function(valArr) {
                return sortedSides = sortedSides || valArr.sort(function(a, b) { return a.outerHeight(true) > b.outerHeight(true) })
            },

            equalSideHeight: function($el) {
                var side0 = $el.find('.Side0'),
                    side1 = $el.find('.Side1'),
                    slide0Height = this.calcSlideSizeInToggle(side0),
                    slide1Height = this.calcSlideSizeInToggle(side1),
                    maxHeight = Math.max(slide0Height, slide1Height);
                $el.find('.flipsides').css({ 'min-height': maxHeight + 'px' });
                side0.add(side1).css({ bottom: 0 });
            },
            calcSlideSizeInToggle: function($slide) {
                var child = $slide.find('>div'),
                    size = 0;
                child.each(function(pos, el) {
                    size += $(el).outerHeight(true);
                })
                size += $slide.outerHeight(true);
                return size;
            },
            equalSideHeightInToggle: function($el) {
                var side0 = $el.find('.Side0'),
                    side1 = $el.find('.Side1'),
                    slide0Height = this.calcSlideSizeInToggle(side0),
                    slide1Height = this.calcSlideSizeInToggle(side1),
                    maxHeight = Math.max(slide0Height, slide1Height);
                $el.find('.flipsides').css({ 'min-height': maxHeight + 'px' });
                side0.add(side1).css({ bottom: 0 });
            }
        },
        sortedSides;

    function detectMobile() {
        return 'ontouchstart' in window;
    }

    function disableEmptyFlips($flip) {
        var emptyPlaceholders = $flip.find(".scEmptyPlaceholder"),
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
            parent = $placeholder.parents(".slide-heading");
            if (parent.length !== 0) {
                h = parseInt(parent.css("font-size")) * 2 || ($placeholder.height() / 3);
                $placeholder.append($("<p>[No components in title section]</p>"));
            } else {
                h = $placeholder.height();
                $placeholder.append($("<p>[No components in content section]</p>"));
            }
            h += "px";
            $placeholder.removeClass("scEmptyPlaceholder");
            $placeholder.css("height", h);

            //set edit here text

        }
    }

    function calcHeightOnResize() {
        var flip = $('.flip.initialized');
        flip.each(function() {
            api.equalSideHeight($(this))
        })
    }

    api.init = function() {
        var flip = $('.flip:not(.initialized)');
        $(window).on('resize', function() {
            calcHeightOnResize();
        });
        flip.each(function() {
            var $flipModule = $(this).find(".flipsides");
            if ($(this).hasClass('flip-hover') && (!detectMobile())) {
                $(this).hover(function() {
                        $(this).addClass('active');
                    },
                    function() {
                        $(this).removeClass('active');
                    });
            } else {
                $(this).on('click', function() {
                    $(this).toggleClass('active');
                });
            }

            $(this).addClass('initialized');
            api.equalSideHeight($(this));
            disableEmptyFlips($flipModule);
        });
    };
    return api;

}(jQuery, document));


XA.register('flip', XA.component.flip);