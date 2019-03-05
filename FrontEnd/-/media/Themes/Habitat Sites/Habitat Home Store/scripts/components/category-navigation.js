$(function() {
    $('.toggle-nav').click(function(e) {
      $(this).toggleClass('active');
      $('.category-navigation-list').toggleClass('active');
  
      e.preventDefault();
    });
  
    var $currentCategoryIdMetaTag = $('meta[name="CurrentCategoryId"]');
    if ($currentCategoryIdMetaTag) {
      var currentCategoryId = $currentCategoryIdMetaTag.attr('content');
      var $currentCategoryLink = $(
        '.category-navigation-list .category-item .category-link[itemid="' + currentCategoryId + '"]'
      );
      if ($currentCategoryLink) {
        $currentCategoryLink.closest('.category-item').addClass('active');
      }
    }
  });
  