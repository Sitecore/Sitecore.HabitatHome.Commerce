$(document).ready(function () {
  var event = isMobile() ? 'touchend' : 'click';
  var $searchForm = $('#SearchForm');
  var $toggleSearch = $('.toggle-search-bar');

  let targetExitedBoundary = function (target) {
    const isSearchBar = $toggleSearch.is($(target));
    const isSearchBarDescendant = $searchForm.has(target).length;
    return !(isSearchBar || isSearchBarDescendant);
  };

  $(window)
    .off(event + '.searchBar')
    .on(event + '.searchBar', function (e) {
      if (targetExitedBoundary(e.target)) {
        $searchForm.removeClass('active');
      }
    });

  $toggleSearch.off(event).on(event, function () {
    $searchForm.toggleClass('active');
  });

  $searchForm.find(':submit').click(function (e) {
    e.preventDefault();
    $(this).attr('disabled', 'disabled');
    $searchForm.submit();
  });
});
