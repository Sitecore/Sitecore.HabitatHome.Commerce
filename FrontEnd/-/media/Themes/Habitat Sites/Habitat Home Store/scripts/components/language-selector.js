$(document).ready(function () {
    var component = $('.cxa-languageselector-component');
    var availableLanguages = $(component).find('.available-languages');
    var languageIcon = $(component).find('.component-content');

    $(languageIcon).mousemove(function () {
        if (!isMobile()) {
            show();
        }
    });

    $(languageIcon).mouseleave(function () {
        hide();
    });

    $(languageIcon).click(function () {
        if ($(availableLanguages).css('display') === 'none') {
            show();
        } else {
            hide();
        }
    });

    function show() {
        availableLanguages.fadeTo("fast", 1);
        availableLanguages.show();
    }

    function hide() {
        availableLanguages.fadeTo("fast", 0);
        availableLanguages.hide();
    }
});