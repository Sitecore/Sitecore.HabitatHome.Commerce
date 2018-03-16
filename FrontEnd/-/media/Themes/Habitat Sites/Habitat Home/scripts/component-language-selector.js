XA.component.languageSelector = (function($) {

    var api = {};

    function readDataAttributes(item) {
        var country = item.data("country-code");

        return "flags-" + country;
    }

    function initLanguageSelector(instance) {
        var el = $(instance),
            header = el.find(".language-selector-select-item"),
            dropDownList = el.find(".language-selector-item-container"),
            dropDownItem = dropDownList.find(".language-selector-item");

        var className = readDataAttributes(header);
        header.find(">a").addClass(className);

        dropDownList.find(".language-selector-item").each(function() {
            className = readDataAttributes($(this));
            $(this).find(">a").addClass(className);
        });

        header.on("click", function() {
            el.toggleClass('open');            
        });

        dropDownItem.on("click", function() {
            var url = $(this).find("a").attr("href");
            window.location.href = url;
        });
    }


    api.init = function() {
        var languageSelector = $(".language-selector:not(.initialized)");

        languageSelector.each(function() {
            initLanguageSelector(this);
            $(this).addClass("initialized");
        });
    };

    return api;
}(jQuery, document));

XA.register("language-selector", XA.component.languageSelector);