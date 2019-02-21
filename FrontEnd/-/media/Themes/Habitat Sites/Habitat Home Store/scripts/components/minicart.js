$(document).ready(function () {
    var basket;
    var miniCart;

    (function init() {
        selectClasses();
    })();

    function selectClasses() {
        basket = $('.cxa-minicart-component').find('.basket');
        miniCart = $(basket).find('.minicart');
    }

    $(basket).mousemove(function () {
        if (!isMobile()) {
            show();
        }
    });

    $(basket).mouseleave(function () {
        hide();
    });

    $(basket).click(function () {
        if (isMobile()) {
            if ($(miniCart).css('display') === 'none') {
                show();
            } else {
                hide();
            }
        }
    });

    function show() {
        selectClasses();
        $(miniCart).fadeTo('fast', 1);
        $(miniCart).show();
    }

    function hide() {
        selectClasses();
        $(miniCart).fadeTo('fast', 0);
        $(miniCart).hide();
    }
});