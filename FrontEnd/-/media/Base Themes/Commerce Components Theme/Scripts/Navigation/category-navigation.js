$(function () {
  setupCXAMobileNavigation();
  setupSXAMobileNavigation();

  var $currentCategoryIdMetaTag = $('meta[name="CurrentCategoryId"]');

  if ($currentCategoryIdMetaTag) {
    var currentCategoryId = $currentCategoryIdMetaTag.attr('content');

    if ($('.cxa-navigationbar-component').length) {
      highlightCXANavigation(currentCategoryId);
    }

    if ($('.cxa-navigation-component').length) {
      clearSXANavigationHighlights();
      highlightSXANavigation(currentCategoryId);
    }
  }

  /**
   * Setup the CXA mobile navigation accordion.
   */
  function setupCXAMobileNavigation() {
    $('.toggle-nav').click(function (e) {
      e.preventDefault();

      var $toggle = $(this);

      $toggle.toggleClass('active')
        .closest('.product-categories-menu')
        .find('.category-navigation-list')
        .toggleClass('active');
    });
  }

  function setupSXAMobileNavigation() {
    $('.navbar-toggler').click(function (e) {
      e.preventDefault();

      $(this).closest('nav')
        .find('.navigation-list')
        .toggleClass('active');
    });
  }

  /**
   * Highlight CXA navigation based on the category identifier given.
   *
   * @param {string} selectedCategoryId - selected category identifier
   */
  function highlightCXANavigation(selectedCategoryId) {
    var $selectedCategoryLink = $('.category-navigation-list .category-item .category-link[itemid="' + selectedCategoryId + '"]');
    if ($selectedCategoryLink) {
      $selectedCategoryLink.closest('.category-item').addClass('active');
    }
  }

  /**
   * Remove highlights for all SXA navigation links
   */
  function clearSXANavigationHighlights() {
    $('.cxa-navigation-component li').removeClass('active');
  }

  /**
   * Highlight SXA navigation based on the category identifier given.
   *
   * @param {string} selectedCategoryId - selected category identifier
   */
  function highlightSXANavigation(selectedCategoryId) {
    var $selectedCategoryLink = $('.navigation-link-id:contains("' + selectedCategoryId + '")');
    if ($selectedCategoryLink) {
      $selectedCategoryLink.closest('li').addClass('active');
    }
  }
});
