$(document).ready(function () {
    var toolbar = $('.cxa-topbarlinks-component')
    var toolbarItems = $(toolbar).find('ul');
    var toolbarContent = $(toolbar).find('.component-content');

    $(toolbarContent).mousemove(function () {
        if (!isMobile()) {
            show();
        }
    });

    $(toolbarContent).mouseleave(function () {
        hide();
    });

    $(toolbarContent).click(function () {
        if ($(toolbarItems).css('display') === 'none') {
            show();
        } else {
            hide();
        }
    });

    function show() {
        $(toolbarItems).fadeTo('fast', 1);
        $(toolbarItems).show();
    }

    function hide() {
        $(toolbarItems).fadeTo('fast', 0);
        $(toolbarItems).hide();
    }
});