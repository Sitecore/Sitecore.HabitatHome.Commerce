$(document).ready(function() {
    if (isMobile()) {
      var $searchForm = $('#SearchForm');
      var $toggleSearch = $('.toggle-search-bar');
  
      function targetExitedBoundary(target) {
        const isSearchBar = $toggleSearch.is($(target));
        const isSearchBarDescendant = $searchForm.has(target).length;
        return !(isSearchBar || isSearchBarDescendant);
      }
  
      $(window)
        .off('touchend.searchBar')
        .on('touchend.searchBar', function(e) {
          if (targetExitedBoundary(e.target)) {
            $searchForm.removeClass('active');
          }
        });
  
      $toggleSearch.off('touchend').on('touchend', function() {
        $searchForm.toggleClass('active');
      });
  
      $searchForm.find(':submit').click(function(e) {
        e.preventDefault();
        $(this).attr('disabled', 'disabled');
        $searchForm.submit();
      });
    }
  });
  