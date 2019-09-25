$(document).ready(function () {
    var searchForm = $('#SearchForm');
    var toggleSearch = $('.toggle-search-bar');

    $(toggleSearch).mouseleave(function (e) {
        hideSearchForm(e);
    });

    $(searchForm).mouseleave(function (e) {
        hideSearchForm(e);
    });

    function hideSearchForm(target) {
        var eleClassName = target.relatedTarget.className;

        if (eleClassName != 'search-textbox') {
            $(searchForm).removeClass('active');
        } else {
            target.preventDefault();
        }
    }

    $(toggleSearch).click(function (e) {
        $(searchForm).toggleClass('active');
    });

    $(searchForm).find(':submit').click(function (event) {
        event.preventDefault();
        $(this).attr('disabled', 'disabled');
        $(searchForm).submit();
    });
});