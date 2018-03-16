XA.component.snippet = (function ($) {

    var pub = {},
        instance;
    var isFlipInside = function (instance) {
        return !!instance.find('.component.flip').length
    }

    function disableEmptySnippet($snippets) {
        var emptyPlaceholders = $snippets.find(".scEmptyPlaceholder"),
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
                if (typeof (placeholderKey) === "string") {
                    editFrame = Sitecore.PageModes.ChromeManager.chromes().filter(function (chrome) {
                        chromeKey = chrome.openingMarker();
                        if (!chromeKey) {
                            return false;
                        }

                        chromeKey = chromeKey.attr("key");
                        return typeof (chromeKey) !== "undefined" && chromeKey.indexOf(placeholderKey) === 0;
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
            parent = $placeholder.parents(".snippet-container");
            h = parseInt(parent.css("font-size")) * 2 || ($placeholder.height() / 3);
            $placeholder.append($("<p>[No components in snippet]</p>"));
            h += "px";
            $placeholder.removeClass("scEmptyPlaceholder");
            $placeholder.css("height", h);

            //set edit here text

        }
    }

    pub.init = function () {
        var $snippets = $(".snippet:not(.initialized)");
        $snippets.each(function () {
            var $tabModule = $(this).find(".snippet-inner");
            instance = $(this);

            disableEmptySnippet($tabModule);
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

XA.register("snippet", XA.component.snippet);