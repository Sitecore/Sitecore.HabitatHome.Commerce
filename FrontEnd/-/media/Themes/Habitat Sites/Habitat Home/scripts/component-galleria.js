XA.component.galleria = (function($) {
    /* global Galleria:false */
    var api = {};

    function checkPageEditor() {
        if ($("body").hasClass("on-page-editor")) {
            return true;
        }

        return false;
    }

    function initGalleria(component, prop) {
        var id = component.find(".gallery-inner").attr("id");
        Galleria.loadTheme(prop.theme);
        Galleria.run("#" + id, prop);
    }

    api.init = function() {
        if (!checkPageEditor()) {
            var gallery = $(".gallery:not(.initialized)");

            gallery.each(function() {
                var properties = $(this).data("properties");
                initGalleria($(this), properties);

                $(this).addClass("initialized");
            });
        }
    };

    return api;
}(jQuery, document));

XA.register("galleria", XA.component.galleria);